namespace ClickView.GoodStuff.Queues.RabbitMq;

public class MessageContext<TData>
{
    private readonly SubscriptionContext _subscriptionContext;

    public MessageContext(TData data, ulong deliveryTag, DateTime timestamp, string id, SubscriptionContext subscriptionContext)
    {
        _subscriptionContext = subscriptionContext;
        Data = data;
        DeliveryTag = deliveryTag;
        Timestamp = timestamp;
        Id = id;
    }

    public TData Data { get; set; }
    public ulong DeliveryTag { get; set; }
    public DateTime Timestamp { get; set; }
    public string Id { get; set; }

    public Task AcknowledgeAsync()
    {
        _subscriptionContext.AcknowledgeAsync(deliveryTag: DeliveryTag);
        return Task.CompletedTask;
    }

    public Task NegativeAcknowledgeAsync(bool requeue = true)
    {
        _subscriptionContext.NegativeAcknowledgeAsync(deliveryTag: DeliveryTag, requeue: requeue);
        return Task.CompletedTask;
    }
}
