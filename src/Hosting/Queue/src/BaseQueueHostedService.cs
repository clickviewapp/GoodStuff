namespace ClickView.GoodStuff.Hosting.Queue;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Queues.RabbitMq;

/// <summary>
/// A base class for all queue hosted services to inherit from
/// </summary>
/// <typeparam name="TOptions"></typeparam>
public abstract class BaseQueueHostedService<TOptions> : IHostedService, IAsyncDisposable
    where TOptions : BaseQueueHostedServiceOptions
{
    private readonly SemaphoreSlim _subscriptionLock = new(1, 1);
    private readonly IQueueClient _queueClient;
    private readonly string _name;
    private SubscriptionContext? _subscriptionContext;
    private bool _disposed;

    /// <summary>
    /// Gets the <see cref="ILogger"/>.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the options.
    /// </summary>
    protected TOptions Options { get; }

    /// <summary>
    /// Initialises a new instance of <see cref="BaseQueueHostedService{TOptions}"/>.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="loggerFactory"></param>
    protected BaseQueueHostedService(IOptions<TOptions> options, ILoggerFactory loggerFactory)
    {
        var type = GetType();

        _name = type.Name;
        Options = options.Value;
        Logger = loggerFactory.CreateLogger(type);

        _queueClient = new RabbitMqClient(CreateOptions(Options, loggerFactory));
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        CheckDisposed();

        await _subscriptionLock.WaitAsync(cancellationToken);

        try
        {
            // If we have a context, don't start again
            if (_subscriptionContext is not null)
                throw new InvalidOperationException($"Cannot start already started service {_name} ");

            var queueName = Options.QueueName;

            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException($"{nameof(QueueHostedServiceOptions.QueueName)} is required");

            _subscriptionContext = await SubscribeAsync(_queueClient, queueName, cancellationToken);
        }
        finally
        {
            _subscriptionLock.Release();
        }

        Logger.QueueServiceStarted(_name);
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        CheckDisposed();

        await _subscriptionLock.WaitAsync(cancellationToken);

        try
        {
            var subContext = _subscriptionContext;

            // Dispose the subscription context to unsubscribe from the queue
            if (subContext is not null)
            {
                await subContext.DisposeAsync();
                _subscriptionContext = null;
            }

            // Call any other code we need to do on stop after the subscription is closed
            await OnStopAsync(cancellationToken);
        }
        finally
        {
            _subscriptionLock.Release();
        }

        Logger.QueueServiceStopped(_name);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the current <see cref="BaseQueueHostedService{TOptions}"/> instance
    /// </summary>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
            return;

        _disposed = true;

        // If we still have an open subscription, Dispose it
        if (_subscriptionContext != null)
            await _subscriptionContext.DisposeAsync();

        // Then finally dispose the client
        await _queueClient.DisposeAsync();

        // Clean up everything else
        _subscriptionLock.Dispose();
    }

    /// <summary>
    /// Creates a subscription context
    /// </summary>
    /// <param name="queueClient"></param>
    /// <param name="queueName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task<SubscriptionContext> SubscribeAsync(IQueueClient queueClient, string queueName,
        CancellationToken cancellationToken);

    /// <summary>
    /// Called when the queue service is stopping and the subscription has been unsubscribed
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task OnStopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>
    /// Acknowledges one or more messages
    /// </summary>
    /// <param name="deliveryTag">The delivery tag of the message to acknowledge</param>
    /// <param name="multiple">If true, acknowledge all outstanding delivery tags up to and including the delivery tag</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected Task AcknowledgeAsync(ulong deliveryTag, bool multiple = false, CancellationToken cancellationToken = default)
    {
        CheckDisposed();

        var subContext = _subscriptionContext;
        if (subContext is null)
            throw new InvalidOperationException("Cannot call acknowledge before starting the worker");

        if (subContext.IsOpen)
            return subContext.AcknowledgeAsync(deliveryTag, multiple, cancellationToken);

        Logger.AcknowledgeFailureChannelNotOpen(deliveryTag);
        return Task.CompletedTask;
    }

    private static RabbitMqClientOptions CreateOptions(BaseQueueHostedServiceOptions options, ILoggerFactory loggerFactory)
    {
        var host = options.Host;

        if (string.IsNullOrWhiteSpace(host))
            throw new ArgumentException($"{nameof(QueueHostedServiceOptions.Host)} is required");

        var o = new RabbitMqClientOptions
        {
            Host = host,
            Port = options.Port,
            Username = options.Username,
            Password = options.Password,
            ConnectionTimeout = options.ConnectionTimeout,
            EnableSsl = options.EnableSsl,
            IgnoreSslErrors = options.IgnoreSslErrors,
            LoggerFactory = loggerFactory
        };

        if (options.SslVersion != null)
            o.SslVersion = options.SslVersion.Value;

        if (options.Serializer != null)
            o.Serializer = options.Serializer;

        return o;
    }

    private void CheckDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().Name);
    }
}
