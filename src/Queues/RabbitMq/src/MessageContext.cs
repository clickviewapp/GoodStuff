namespace ClickView.GoodStuff.Queues.RabbitMq;

public class MessageContext<TData>
{
    private readonly SubscriptionContext _subscriptionContext;

    internal MessageContext(TData data, ulong deliveryTag, DateTime timestamp, string id, MessagePriority priority,
        SubscriptionContext subscriptionContext, bool reDelivered)
    {
        _subscriptionContext = subscriptionContext;
        ReDelivered = reDelivered;
        Data = data;
        DeliveryTag = deliveryTag;
        Timestamp = timestamp;
        Id = id;
        Priority = priority;
    }

    /// <summary>
    /// The message data
    /// </summary>
    public TData Data { get; }

    /// <summary>
    /// The RabbitMQ delivery tag
    /// </summary>
    public ulong DeliveryTag { get; }

    /// <summary>
    /// The timestamp the message was enqueued
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// The unique ID for this message
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The priority of the message
    /// </summary>
    public MessagePriority Priority { get; }

    /// <summary>
    /// True if the message has been re-delivered
    /// </summary>
    public bool ReDelivered { get; }

    /// <summary>
    /// Returns true if the RabbitMQ channel is open
    /// </summary>
    public bool IsOpen => _subscriptionContext.IsOpen;

    /// <summary>
    /// Acknowledges one or more messages
    /// </summary>
    /// <param name="multiple">If true, acknowledge all outstanding delivery tags up to and including the delivery tag</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public ValueTask AcknowledgeAsync(bool multiple = false, CancellationToken cancellationToken = default)
    {
        return _subscriptionContext.AcknowledgeAsync(
            deliveryTag: DeliveryTag,
            multiple: multiple,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Rejects one or more messages
    /// </summary>
    /// <param name="multiple">If true, reject all outstanding delivery tags up to and including the delivery tag</param>
    /// <param name="requeue">If true, requeue the delivery (or multiple deliveries if <paramref name="multiple"/> is true)) with the specified delivery tag</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public ValueTask NegativeAcknowledgeAsync(bool multiple = false, bool requeue = true, CancellationToken cancellationToken = default)
    {
        return _subscriptionContext.NegativeAcknowledgeAsync(
            deliveryTag: DeliveryTag,
            multiple: multiple,
            requeue: requeue,
            cancellationToken: cancellationToken);
    }
}
