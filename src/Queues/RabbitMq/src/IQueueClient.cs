namespace ClickView.GoodStuff.Queues.RabbitMq;

public interface IQueueClient : IAsyncDisposable
{
    Task EnqueueAsync<TData>(string exchange, TData data, EnqueueOptions? options = null,
        CancellationToken cancellationToken = default);

    Task<SubscriptionContext> SubscribeAsync<TData>(string queue,
        Func<MessageContext<TData>, CancellationToken, Task> callback,
        SubscribeOptions? options = null,
        CancellationToken cancellationToken = default);

    Task UnsubscribeAllAsync(CancellationToken cancellationToken = default);
}
