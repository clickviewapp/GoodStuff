namespace ClickView.GoodStuff.Queues.RabbitMq;

using System.Net.Security;
using Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

public class RabbitMqClient : IQueueClient, IAsyncDisposable
{
    private readonly RabbitMqClientOptions _options;
    private readonly ConnectionFactory _connectionFactory;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private readonly ActiveSubscriptions _activeSubscriptions = new();
    private readonly ILogger<RabbitMqClient> _logger;

    private IConnection? _connection;
    private bool _disposed;

    public RabbitMqClient(IOptions<RabbitMqClientOptions> options)
    {
        _options = options.Value;
        _logger = _options.LoggerFactory.CreateLogger<RabbitMqClient>();
        _connectionFactory = CreateConnectionFactory(_options);
    }

    public async Task EnqueueAsync<TData>(string exchange, TData data, EnqueueOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(exchange);
        ArgumentNullException.ThrowIfNull(data);

        CheckDisposed();

        // If no options are passed in, use the defaults
        options ??= EnqueueOptions.Default;

        // Serialize the data before we try to connect so we throw any exceptions early
        var message = MessageWrapper<TData>.New(data);
        var bytes = _options.Serializer.Serialize(message);

        using var channel = await GetChannelAsync(cancellationToken);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = options.Persistent;

        channel.BasicPublish(exchange, options.RoutingKey, properties, bytes);
    }

    public async Task<SubscriptionContext> SubscribeAsync<TData>(string queue,
        Func<MessageContext<TData>, CancellationToken, Task> callback,
        SubscribeOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queue);
        ArgumentNullException.ThrowIfNull(callback);

        CheckDisposed();

        // If no options are passed in, use the defaults
        options ??= SubscribeOptions.Default;

        // Don't dispose channel, dispose the SubscriptionContext instead
        var channel = await GetChannelAsync(cancellationToken);

        try
        {
            var shutdownTaskWaiter = new CountWaiter();

            var subContext = new SubscriptionContext(
                queueName: queue,
                channel: channel,
                subscriptions: _activeSubscriptions,
                taskWaiter: shutdownTaskWaiter,
                logger: _options.LoggerFactory.CreateLogger<SubscriptionContext>());

            var consumer = new RabbitMqCallbackConsumer<TData>(
                subContext,
                callback,
                shutdownTaskWaiter,
                _options.Serializer,
                _options.LoggerFactory.CreateLogger<RabbitMqCallbackConsumer<TData>>()
            );

            channel.BasicQos(0, options.PrefetchCount, false);

            var consumerTag = channel.BasicConsume(
                queue: queue,
                autoAck: options.AutoAcknowledge,
                consumer: consumer);

            subContext.SetConsumerTag(consumerTag);

            // track our active subscriptions
            _activeSubscriptions.Add(subContext);

            _logger.LogDebug("Successfully subscribed to queue {QueueName}", queue);

            return subContext;
        }
        catch
        {
            // on any exception, dispose the channel so we dont leak memory
            channel.Dispose();

            throw;
        }
    }

    public async Task UnsubscribeAllAsync(CancellationToken cancellationToken = default)
    {
        // Dispose all active subs
        var contexts = _activeSubscriptions.GetAll();

        if (contexts.Count == 0)
            return;

        _logger.LogDebug("Unsubscribing {Count} listeners", contexts.Count);

        var exceptions = new List<Exception>();

        foreach (var context in contexts)
        {
            try
            {
                await context.DisposeAsync();
                _activeSubscriptions.Remove(context);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count > 0)
            throw new AggregateException("Failed to unsubscribe from queues", exceptions);
    }

    private ValueTask<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        CheckDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        var connection = _connection;
        if (connection is not null)
            return new ValueTask<IConnection>(connection);

        return ConnectSlowAsync(cancellationToken);
    }

    private async ValueTask<IConnection> ConnectSlowAsync(CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);

        try
        {
            if (_connection is not null)
                return _connection;

            _logger.LogDebug("Connecting to RabbitMQ...");

            // Create a new connection
            var connection = _connectionFactory.CreateConnection();

            // Setup logging
            connection.CallbackException += (_, args) =>
                _logger.LogError(args.Exception, "Exception thrown in RabbitMQ callback");
            connection.ConnectionBlocked += (_, _) => _logger.LogDebug("RabbitMQ connection blocked");
            connection.ConnectionUnblocked += (_, _) => _logger.LogDebug("RabbitMQ connection unblocked");
            connection.ConnectionShutdown += (_, _) => _logger.LogDebug("RabbitMQ connection shutdown");

            if (connection is IAutorecoveringConnection autorecoveringConnection)
            {
                autorecoveringConnection.RecoveringConsumer += (_, _) => _logger.LogDebug("RecoveringConsumer");
                autorecoveringConnection.RecoverySucceeded += (_, _) => _logger.LogDebug("RecoverySucceeded");
                autorecoveringConnection.ConnectionRecoveryError += (_, args) =>
                    _logger.LogError(args.Exception, "ConnectionRecoveryError");
            }

            _connection = connection;

            _logger.LogDebug("Connection to RabbitMQ successful");

            return connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private async ValueTask<IModel> GetChannelAsync(CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionAsync(cancellationToken);
        return connection.CreateModel();
    }

    private void CheckDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().Name);
    }

    private static ConnectionFactory CreateConnectionFactory(RabbitMqClientOptions options)
    {
        var factory = new ConnectionFactory
        {
            HostName = options.Host,
            Port = options.Port,
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true
        };

        // Username
        if (!string.IsNullOrEmpty(options.Username))
            factory.UserName = options.Username;

        // Password
        if (!string.IsNullOrEmpty(options.Password))
            factory.Password = options.Password;

        // RequestedConnectionTimeout
        if (options.ConnectionTimeout.HasValue)
            factory.RequestedConnectionTimeout = options.ConnectionTimeout.Value;

        // SSL
        factory.Ssl.Enabled = options.EnableSsl;

        if (options.SslProtocols.HasValue)
            factory.Ssl.Version = options.SslProtocols.Value;

        return factory;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        await UnsubscribeAllAsync();

        _disposed = true;
        _connectionLock.Dispose();
        _connection?.Dispose();
    }
}
