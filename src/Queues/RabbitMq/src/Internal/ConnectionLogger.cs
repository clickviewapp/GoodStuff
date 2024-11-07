namespace ClickView.GoodStuff.Queues.RabbitMq.Internal;

using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

internal class ConnectionLogger
{
    private readonly ILogger _logger;

    public ConnectionLogger(IConnection connection, ILogger logger)
    {
        _logger = logger;

        connection.CallbackExceptionAsync += CallbackExceptionAsync;
        connection.ConnectionShutdownAsync += ConnectionOnConnectionShutdownAsync;
        connection.RecoverySucceededAsync += ConnectionOnRecoverySucceededAsync;
        connection.ConnectionRecoveryErrorAsync += ConnectionOnConnectionRecoveryErrorAsync;
        connection.ConsumerTagChangeAfterRecoveryAsync += ConnectionOnConsumerTagChangeAfterRecoveryAsync;
        connection.QueueNameChangedAfterRecoveryAsync += ConnectionOnQueueNameChangedAfterRecoveryAsync;
        connection.RecoveringConsumerAsync += ConnectionOnRecoveringConsumerAsync;
        connection.ConnectionBlockedAsync += ConnectionOnConnectionBlockedAsync;
        connection.ConnectionUnblockedAsync += ConnectionOnConnectionUnblockedAsync;
    }

    private Task CallbackExceptionAsync(object sender, CallbackExceptionEventArgs args)
    {
        _logger.LogError(args.Exception, "Exception thrown in RabbitMQ callback");
        return Task.CompletedTask;
    }

    private Task ConnectionOnConnectionShutdownAsync(object sender, ShutdownEventArgs args)
    {
        _logger.LogDebug("RabbitMQ connection shutdown");
        return Task.CompletedTask;
    }

    private Task ConnectionOnRecoverySucceededAsync(object sender, AsyncEventArgs args)
    {
        _logger.LogDebug("RabbitMQ connection recovery succeeded");
        return Task.CompletedTask;
    }

    private Task ConnectionOnConnectionRecoveryErrorAsync(object sender, ConnectionRecoveryErrorEventArgs args)
    {
        _logger.LogDebug("RabbitMQ connection recovery error");
        return Task.CompletedTask;
    }

    private Task ConnectionOnConsumerTagChangeAfterRecoveryAsync(object sender,
        ConsumerTagChangedAfterRecoveryEventArgs args)
    {
        _logger.LogDebug("RabbitMQ connection consumer tag changed after recovery");
        return Task.CompletedTask;
    }

    private Task ConnectionOnQueueNameChangedAfterRecoveryAsync(object sender,
        QueueNameChangedAfterRecoveryEventArgs args)
    {
        _logger.LogDebug("RabbitMQ connection queue name changed after recovery");
        return Task.CompletedTask;
    }

    private Task ConnectionOnRecoveringConsumerAsync(object sender, RecoveringConsumerEventArgs args)
    {
        _logger.LogDebug("RabbitMQ connection recovering consumer");
        return Task.CompletedTask;
    }

    private Task ConnectionOnConnectionBlockedAsync(object sender, ConnectionBlockedEventArgs args)
    {
        _logger.LogDebug("RabbitMQ connection blocked");
        return Task.CompletedTask;
    }

    private Task ConnectionOnConnectionUnblockedAsync(object sender, AsyncEventArgs args)
    {
        _logger.LogDebug("RabbitMQ connection unblocked");
        return Task.CompletedTask;
    }
}
