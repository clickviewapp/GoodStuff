namespace ClickView.GoodStuff.Queues.RabbitMq;

public class MessageContext<TData>
{
    private readonly SubscriptionContext _subscriptionContext;

    internal MessageContext(TData data, ulong deliveryTag, DateTime timestamp, string id, SubscriptionContext subscriptionContext)
    {
        _subscriptionContext = subscriptionContext;
        Data = data;
        DeliveryTag = deliveryTag;
        Timestamp = timestamp;
        Id = id;
    }

    /// <summary>
    /// The message data
    /// </summary>
    public TData Data { get; set; }

    /// <summary>
    /// The RabbitMQ delivery tag
    /// </summary>
    public ulong DeliveryTag { get; set; }

    /// <summary>
    /// The timestamp the message was enqueued
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// The unique ID for this message
    /// </summary>
    public string Id { get; set; }

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
    public Task AcknowledgeAsync(bool multiple = false, CancellationToken cancellationToken = default)
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
    /// <param name="requeue">If true, requeue the delivery (or multiple deliveries if <see cref="multiple"/> is true)) with the specified delivery tag</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public Task NegativeAcknowledgeAsync(bool multiple = false, bool requeue = true, CancellationToken cancellationToken = default)
    {
        return _subscriptionContext.NegativeAcknowledgeAsync(
            deliveryTag: DeliveryTag,
            multiple: multiple,
            requeue: requeue,
            cancellationToken: cancellationToken);
    }
}
