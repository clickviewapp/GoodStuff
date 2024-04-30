namespace ClickView.GoodStuff.Hosting.Queue;

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Queues.RabbitMq;

/// <summary>
/// A hosted service that connects to a queue and buffers messages before sending them to <see cref="OnMessagesAsync"/>
/// </summary>
/// <typeparam name="TMessage"></typeparam>
/// <typeparam name="TOptions"></typeparam>
public abstract class BatchQueueHostedService<TMessage, TOptions> : BaseQueueHostedService<TOptions>
    where TOptions : BatchQueueHostedServiceOptions
{
    // Flush interval
    private Task? _workerTask;
    private CancellationTokenSource? _cts;

    // Buffer processing
    private readonly List<TMessage> _currentBuffer;
    private readonly object _bufferLock = new();
    private readonly SemaphoreSlim _workerLock = new(1, 1);
    private ulong _lastDeliveryTag;
    private DateTime? _lastMessage;
    private DateTime _lastProcess = DateTime.UtcNow;

    /// <summary>
    /// Initialises a new instance of <see cref="QueueHostedService{TMessage,TOptions}"/>.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="loggerFactory"></param>
    protected BatchQueueHostedService(IOptions<TOptions> options, ILoggerFactory loggerFactory) : base(options, loggerFactory)
    {
        _currentBuffer = new List<TMessage>(Options.BatchSize);
    }

    /// <inheritdoc />
    protected override async Task<SubscriptionContext> SubscribeAsync(IQueueClient queueClient, string queueName, CancellationToken cancellationToken)
    {
        // Validate options
        var minFlushInterval = Options.MinFlushInterval;
        var maxFlushInterval = Options.MaxFlushInterval;

        if (maxFlushInterval < minFlushInterval)
            throw new Exception(
                $"{nameof(Options.MaxFlushInterval)} must be greater than {nameof(Options.MinFlushInterval)}");

        var subscriptionContext = await queueClient.SubscribeAsync<TMessage>(queueName, OnMessageAsync,
            new SubscribeOptions
            {
                PrefetchCount = Options.BatchSize,
                AutoAcknowledge = false
            }, cancellationToken);

        // Start background worker task if we have a minimum or a maximum flush interval
        if (minFlushInterval > TimeSpan.Zero || maxFlushInterval > TimeSpan.Zero)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _workerTask = IntervalWorkerAsync(_cts.Token);
        }

        return subscriptionContext;
    }

    /// <inheritdoc />
    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        // Stop the worker task and wait for it to finish if it exists
        _cts?.Cancel();
        var workerTask = _workerTask;
        if (workerTask is not null)
        {
            try
            {
                await workerTask;
            }
            catch (OperationCanceledException) when (_cts!.IsCancellationRequested)
            {
                // ignore the cancelled task exception when it was us that cancelled the task
            }
        }
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
         await base.DisposeAsyncCore();

         _cts?.Dispose();
         _workerLock.Dispose();
         _workerTask = null;
    }

    /// <summary>
    /// This method is called when a batch of messages have been received from the configured queue.
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task OnMessagesAsync(IEnumerable<TMessage> messages,
        CancellationToken cancellationToken = default);

    private async Task OnMessageAsync(MessageContext<TMessage> messageContext, CancellationToken cancellationToken)
    {
        Logger.QueueMessageReceived(messageContext.Id, messageContext.Timestamp);

        List<TMessage> snapshot;
        var batchSize = Options.BatchSize;

        lock (_bufferLock)
        {
            // Update last message time
            _lastMessage = Now();

            // Add message into buffer
            _currentBuffer.Add(messageContext.Data);
            _lastDeliveryTag = messageContext.DeliveryTag;

            // If our buffer is at max size then we need to process the items
            if (_currentBuffer.Count < batchSize)
                return;

            // Take a snapshot of the buffer and clear the current buffer data
            snapshot = _currentBuffer.ToList();
            _currentBuffer.Clear();
            _lastDeliveryTag = 0;
        }

        await ProcessItemsAsync(snapshot, messageContext.DeliveryTag, FlushReason.BufferFull, cancellationToken);
    }

    /// <summary>
    /// Task which loops on an interval to check if we need to process an items
    /// </summary>
    /// <param name="cancellationToken"></param>
    private async Task IntervalWorkerAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var minFlushInterval = Options.MinFlushInterval;
                var maxFlushInterval = Options.MaxFlushInterval;

                var hashMinFlush = minFlushInterval > TimeSpan.Zero;
                var hasMaxFlush = maxFlushInterval > TimeSpan.Zero;

                ulong latestDeliveryTag;
                IReadOnlyCollection<TMessage> snapshot;
                TimeSpan timeSinceLastMessage;
                TimeSpan timeSinceLastProcess;
                DateTime lastProcess;
                FlushReason? reason = null;

                var waitTime = minFlushInterval;

                lock (_bufferLock)
                {
                    var now = Now();
                    bool snapshotBuffer;

                    lastProcess = _lastProcess;
                    timeSinceLastMessage = _lastMessage.HasValue ? now - _lastMessage.Value : TimeSpan.Zero;
                    timeSinceLastProcess = now - lastProcess;

                    // We only need to do work if the buffer contains any items
                    if (_currentBuffer.Count == 0)
                    {
                        snapshotBuffer = false;
                    }
                    else
                    {
                        // First check to make sure that we haven't waited over our maximum allowed time since last process
                        if (hasMaxFlush && timeSinceLastProcess > maxFlushInterval)
                        {
                            snapshotBuffer = true;
                            reason = FlushReason.MaxIntervalReached;
                        }
                        // Check if we received a message recently and if we have then wait until the next interval
                        else if (hashMinFlush && timeSinceLastMessage < minFlushInterval)
                        {
                            snapshotBuffer = false;

                            // Because Task.Delay is not accurate and may wait shorter than required,
                            // we'll add an extra second to the wait time to prevent spamming
                            waitTime = (minFlushInterval - timeSinceLastMessage) + TimeSpan.FromSeconds(1);
                        }
                        else
                        {
                            snapshotBuffer = true;
                            reason = FlushReason.IntervalReached;
                        }
                    }

                    if (snapshotBuffer)
                    {
                        // Take a copy of the buffer and reset
                        snapshot = _currentBuffer.ToList();
                        latestDeliveryTag = _lastDeliveryTag;
                        _currentBuffer.Clear();
                        _lastDeliveryTag = 0;
                    }
                    else
                    {
                        snapshot = Array.Empty<TMessage>();
                        latestDeliveryTag = 0;
                    }
                }

                if (snapshot.Count > 0)
                {
                    Debug.Assert(reason.HasValue);

                    Logger.LogLastProcessTime(lastProcess, timeSinceLastProcess);
                    await ProcessItemsAsync(snapshot, latestDeliveryTag, reason!.Value, cancellationToken);
                }

                Logger.BatchProcessIntervalWait(waitTime, timeSinceLastMessage);
                await Task.Delay(waitTime, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Propagate up to caller to handle
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected exception thrown when running interval check");

                // Wait before trying again
                await Task.Delay(Options.MinFlushInterval, cancellationToken);
            }
        }
    }

    private async Task ProcessItemsAsync(IReadOnlyCollection<TMessage> messages,
        ulong latestDeliveryTag,
        FlushReason reason,
        CancellationToken cancellationToken)
    {
        await _workerLock.WaitAsync(cancellationToken);

        try
        {
            Logger.BatchProcessStart(messages.Count, reason);

            var sw = Stopwatch.StartNew();

            await OnMessagesAsync(messages, cancellationToken);

            Logger.BatchProcessCompleted(messages.Count, sw.Elapsed);
        }
        catch (OperationCanceledException)
        {
            // Propagate up to caller to handle
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unhandled exception caught when processing message");
        }
        finally
        {
            // Finally, acknowledge the message
            // Another try here to make sure we release the workerLock if anything goes wrong when acknowledging
            try
            {
                await AcknowledgeAsync(latestDeliveryTag, true, cancellationToken);
            }
            finally
            {
                // Use the buffer lock again to make sure the last Process is not updated by multiple threads
                lock (_bufferLock)
                    _lastProcess = Now();

                _workerLock.Release();
            }
        }
    }

    private static DateTime Now() => DateTime.UtcNow;
}
