namespace ClickView.GoodStuff.Queues.RabbitMq;

public interface IQueueClient : IAsyncDisposable
{
    /// <summary>
    /// Connects to the Queue.
    /// This is not required as the client will auto connect, but is useful for forcing a connection.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Enqueues a message into an exchange.
    /// </summary>
    /// <param name="exchange">The name of the exchange</param>
    /// <param name="data">The data to enqueue</param>
    /// <param name="options">Additional enqueue options</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TData"></typeparam>
    /// <returns></returns>
    Task EnqueueAsync<TData>(string exchange, TData data, EnqueueOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to a queue.
    /// </summary>
    /// <param name="queue">The name of the queue</param>
    /// <param name="callback">A callback function which is called when a new message is received</param>
    /// <param name="options">Additional subscription options</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TData"></typeparam>
    /// <returns>A subscription context. Dispose this to unsubscribe from the queue</returns>
    Task<SubscriptionContext> SubscribeAsync<TData>(string queue,
        Func<MessageContext<TData>, CancellationToken, Task> callback,
        SubscribeOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unsubscribes all active subscriptions.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnsubscribeAllAsync(CancellationToken cancellationToken = default);
}
