namespace ClickView.GoodStuff.Hosting.Queue;

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Queues.RabbitMq;

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
    private DateTime _lastMessage = DateTime.MinValue;
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

    private async Task OnMessageAsync(MessageContext<TMessage> context, CancellationToken cancellationToken)
    {
        Logger.QueueMessageReceived();

        List<TMessage> snapshot;
        var batchSize = Options.BatchSize;

        lock (_bufferLock)
        {
            // Update last message time
            _lastMessage = Now();

            // Add message into buffer
            _currentBuffer.Add(context.Data);
            _lastDeliveryTag = context.DeliveryTag;

            // If our buffer is at max size then we need to process the items
            if (_currentBuffer.Count < batchSize)
                return;

            snapshot = _currentBuffer.ToList();
            _currentBuffer.Clear();
            _lastDeliveryTag = 0;
        }

        Logger.BatchProcessMessageBufferReached(batchSize);

        await ProcessItemsAsync(snapshot, context.DeliveryTag, cancellationToken);
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
                var now = Now();
                var minFlushInterval = Options.MinFlushInterval;
                var maxFlushInterval = Options.MaxFlushInterval;

                var timeSinceLastMessage = now - _lastMessage;
                var timeSinceLastProcess = now - _lastProcess;

                // First check to make sure that we haven't waited over our maximum allowed time since last process
                if (maxFlushInterval > TimeSpan.Zero && timeSinceLastProcess > maxFlushInterval)
                {
                    Logger.BatchProcessMaxFlushIntervalReached(timeSinceLastProcess);
                }
                // Check if we received a message recently and if we have then wait until the next interval
                else if (minFlushInterval > TimeSpan.Zero && timeSinceLastMessage < minFlushInterval)
                {
                    var waitTime = minFlushInterval - timeSinceLastMessage;

                    // Because Task.Delay is not accurate and may wait shorter than required,
                    // we'll add an extra second to the wait time to prevent spamming
                    waitTime += TimeSpan.FromSeconds(1);

                    Logger.BatchProcessMinFlushIntervalNotReached(timeSinceLastMessage, waitTime);

                    await Task.Delay(waitTime, cancellationToken);

                    continue;
                }

                // We may need to process the items
                // Lets check the message size and if we have any items then process

                ulong latestDeliveryTag;
                IReadOnlyCollection<TMessage> snapshot;

                lock (_bufferLock)
                {
                    // Nothing in the buffer, therefore nothing to do
                    if (_currentBuffer.Count == 0)
                    {
                        snapshot = Array.Empty<TMessage>();
                        latestDeliveryTag = 0;
                    }
                    else
                    {
                        // Take a copy of the buffer and reset
                        snapshot = _currentBuffer.ToList();
                        latestDeliveryTag = _lastDeliveryTag;
                        _currentBuffer.Clear();
                        _lastDeliveryTag = 0;
                    }
                }

                // Same check exists in ProcessItemsAsync but im doing it here as well to avoid the await overhead
                // if we have 0 items
                if (snapshot.Count > 0)
                {
                    Logger.BatchProcessIntervalReached(_lastProcess);
                    await ProcessItemsAsync(snapshot, latestDeliveryTag, cancellationToken);
                }
                else
                {
                    _lastProcess = Now();
                }

                Logger.BatchProcessIntervalWait(minFlushInterval);
                await Task.Delay(minFlushInterval, cancellationToken);
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
        CancellationToken cancellationToken)
    {
        if (messages.Count == 0)
        {
            // If there's noting to process, update the process time anyway
            // We do this because if we've not received any items it will have a last process time
            // in the past which will force the first check to always process whatever is in the buffer
            _lastProcess = Now();
            return;
        }

        await _workerLock.WaitAsync(cancellationToken);

        try
        {
            Logger.BatchProcessStart(messages.Count);

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
                _lastProcess = Now();
                _workerLock.Release();
            }
        }
    }

    private static DateTime Now() => DateTime.UtcNow;
}
