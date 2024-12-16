namespace ClickView.GoodStuff.Queues.RabbitMq;

using System.Net.Security;
using Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

public class RabbitMqClient : IQueueClient
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

    /// <inheritdoc />
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        await GetConnectionAsync(cancellationToken);
    }

    /// <inheritdoc />
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

        await using var channel = await GetChannelAsync(options.EnablePublisherConfirms, cancellationToken);

        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: options.RoutingKey,
            mandatory: true,
            basicProperties: new BasicProperties
            {
                Persistent = options.Persistent
            },
            body: bytes,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
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

        // We don't want to dispose the channel here (unless an exception is thrown, see below).
        // The returned SubscriptionContext is the object that should be disposed (which disposes the channel)
        var channel = await GetChannelAsync(false, cancellationToken);

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
                channel,
                subContext,
                callback,
                shutdownTaskWaiter,
                _options.Serializer,
                _options.LoggerFactory.CreateLogger<RabbitMqCallbackConsumer<TData>>()
            );

            await channel.BasicQosAsync(0, options.PrefetchCount, false, cancellationToken);

            var consumerTag = await channel.BasicConsumeAsync(
                queue: queue,
                autoAck: options.AutoAcknowledge,
                consumer: consumer,
                cancellationToken: cancellationToken);

            subContext.SetConsumerTag(consumerTag);

            // track our active subscriptions
            _activeSubscriptions.Add(subContext);

            _logger.SubscribedToQueue(queue);

            return subContext;
        }
        catch
        {
            // On any exception, dispose the channel so we dont leak memory
            channel.Dispose();

            throw;
        }
    }

    /// <inheritdoc />
    public async Task UnsubscribeAllAsync(CancellationToken cancellationToken = default)
    {
        CheckDisposed();

        // Dispose all active subs
        var contexts = _activeSubscriptions.GetAll();

        if (contexts.Count == 0)
            return;

        _logger.UnsubscribingListeners(contexts.Count);

        // Unsubscribe all at once
        await Task.WhenAll(contexts.Select(async c =>
        {
            await c.DisposeAsync();
            _activeSubscriptions.Remove(c);
        }));
    }

    private ValueTask<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        CheckDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        var connection = _connection;
        if (connection is not null)
            return new ValueTask<IConnection>(connection);

        return ConnectSlowAsync(cancellationToken);
    }

    private async ValueTask<IConnection> ConnectSlowAsync(CancellationToken cancellationToken)
    {
        await _connectionLock.WaitAsync(cancellationToken);

        try
        {
            if (_connection is not null)
                return _connection;

            _logger.ConnectingToRabbitMq();

            // Create a new connection
            var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

            // Setup logging
            _ = new ConnectionLogger(connection, _logger);

            _connection = connection;

            _logger.ConnectedToRabbitMq();

            return connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private async Task<IChannel> GetChannelAsync(bool enablePublisherConfirms, CancellationToken cancellationToken)
    {
        var connection = await GetConnectionAsync(cancellationToken);

        var options = new CreateChannelOptions(
            publisherConfirmationsEnabled: enablePublisherConfirms,
            publisherConfirmationTrackingEnabled: enablePublisherConfirms
        );

        return await connection.CreateChannelAsync(options, cancellationToken);
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
            AutomaticRecoveryEnabled = true,
            ConsumerDispatchConcurrency = options.ConsumerDispatchConcurrency
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
        factory.Ssl.Version = options.SslVersion;

        if (options.IgnoreSslErrors)
        {
            factory.Ssl.AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNotAvailable |
                                                 SslPolicyErrors.RemoteCertificateChainErrors |
                                                 SslPolicyErrors.RemoteCertificateNameMismatch;
        }

        return factory;
    }

    /// <inheritdoc />
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
