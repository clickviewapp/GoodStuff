namespace ClickView.GoodStuff.Queues.RabbitMq;

public class MessageContext<TData>
{
    private readonly SubscriptionContext _subscriptionContext;

    internal MessageContext(
        TData data,
        string routingKey,
        ulong deliveryTag,
        DateTime timestamp,
        string id,
        MessagePriority priority,
        SubscriptionContext subscriptionContext,
        bool reDelivered,
        long? deliveryCount)
    {
        Data = data;
        RoutingKey = routingKey;
        DeliveryTag = deliveryTag;
        Timestamp = timestamp;
        Id = id;
        Priority = priority;
        _subscriptionContext = subscriptionContext;
        ReDelivered = reDelivered;
        DeliveryCount = deliveryCount;
    }

    /// <summary>
    /// The message data
    /// </summary>
    public TData Data { get; }

    /// <summary>
    /// The routing key of the message. If not set, this will be an empty string.
    /// </summary>
    public string RoutingKey { get; }

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
    /// The number of unsuccessful (re)delivery attempts (only for quorum queues)
    /// </summary>
    public long? DeliveryCount { get; }

    /// <summary>
    /// Returns true if the RabbitMQ channel is open
    /// </summary>
    public bool IsOpen => _subscriptionContext.IsOpen;

    /// <summary>
    /// Returns true if the message has been acknowledged (either ack or nack).
    /// </summary>
    public bool Acknowledged { get; private set; }

    /// <summary>
    /// Acknowledges one or more messages
    /// </summary>
    /// <param name="multiple">If true, acknowledge all outstanding delivery tags up to and including the delivery tag</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public async ValueTask AcknowledgeAsync(bool multiple = false, CancellationToken cancellationToken = default)
    {
        CheckAcknowledged();

        await _subscriptionContext.AcknowledgeAsync(
            deliveryTag: DeliveryTag,
            multiple: multiple,
            cancellationToken: cancellationToken);

        Acknowledged = true;
    }

    /// <summary>
    /// Rejects one or more messages
    /// </summary>
    /// <param name="multiple">If true, reject all outstanding delivery tags up to and including the delivery tag</param>
    /// <param name="requeue">If true, requeue the delivery (or multiple deliveries if <paramref name="multiple"/> is true)) with the specified delivery tag</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public async ValueTask NegativeAcknowledgeAsync(bool multiple = false, bool requeue = true, CancellationToken cancellationToken = default)
    {
        CheckAcknowledged();

        await _subscriptionContext.NegativeAcknowledgeAsync(
            deliveryTag: DeliveryTag,
            multiple: multiple,
            requeue: requeue,
            cancellationToken: cancellationToken);

        Acknowledged = true;
    }

    private void CheckAcknowledged()
    {
        if (Acknowledged)
            throw new InvalidOperationException("Message has already been Acknowledged.");
    }
}
