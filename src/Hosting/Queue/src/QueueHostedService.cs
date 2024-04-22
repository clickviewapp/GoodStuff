namespace ClickView.GoodStuff.Hosting.Queue;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Queues.RabbitMq;

/// <summary>
/// A hosted service that connects to a queue and executes <see cref="OnMessageAsync(TMessage,System.Threading.CancellationToken)" />
/// when a message is received
/// </summary>
/// <typeparam name="TMessage"></typeparam>
/// <typeparam name="TOptions"></typeparam>
public abstract class QueueHostedService<TMessage, TOptions> : IHostedService, IAsyncDisposable
    where TMessage : class, new()
    where TOptions : QueueHostedServiceOptions
{
    private readonly SemaphoreSlim _subscriptionLock = new(1, 1);
    private readonly IQueueClient _queueClient;
    private readonly string _name;
    private SubscriptionContext? _subscriptionContext;
    private bool _disposed;

    /// <summary>
    /// Gets the <see cref="ILogger"/>.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Gets the options.
    /// </summary>
    protected readonly TOptions Options;

    /// <summary>
    /// Initialises a new instance of <see cref="QueueHostedService{TMessage,TOptions}"/>.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="loggerFactory"></param>
    protected QueueHostedService(IOptions<TOptions> options, ILoggerFactory loggerFactory)
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

            _subscriptionContext = await _queueClient.SubscribeAsync<TMessage>(queueName,
                OnMessageAsync,
                new SubscribeOptions {PrefetchCount = Options.ConcurrentTaskCount},
                cancellationToken);
        }
        finally
        {
            _subscriptionLock.Release();
        }

        Logger.LogInformation("Queue service started - {Name}", _name);
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

            if (subContext is not null)
            {
                await subContext.DisposeAsync();
                _subscriptionContext = null;
            }
        }
        finally
        {
            _subscriptionLock.Release();
        }

        Logger.LogInformation("Queue service stopped - {Name}", _name);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
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
    /// This method is called when a message has been received from the configured queue.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract Task OnMessageAsync(TMessage message, CancellationToken token);

    private static RabbitMqClientOptions CreateOptions(QueueHostedServiceOptions options, ILoggerFactory loggerFactory)
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

    /// <summary>
    /// Triggered when a message is received from the configured queue.
    /// </summary>>
    /// <param name="messageContext"></param>
    /// <param name="cancellationToken"></param>
    private async Task OnMessageAsync(MessageContext<TMessage> messageContext, CancellationToken cancellationToken)
    {
        Logger.LogDebug("Queue message received");

        try
        {
            await OnMessageAsync(messageContext.Data, cancellationToken);
        }
        finally
        {
            if (messageContext.IsOpen)
            {
                await messageContext.AcknowledgeAsync();
            }
            else
            {
                Logger.LogWarning("Cannot acknowledge task. Channel is not open");
            }
        }
    }

    private void CheckDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().Name);
    }
}
