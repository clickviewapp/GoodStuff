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
        return _subscriptionContext.AcknowledgeAsync(deliveryTag: DeliveryTag);
    }

    public Task NegativeAcknowledgeAsync(bool requeue = true)
    {
        return _subscriptionContext.NegativeAcknowledgeAsync(deliveryTag: DeliveryTag, requeue: requeue);
    }
}
