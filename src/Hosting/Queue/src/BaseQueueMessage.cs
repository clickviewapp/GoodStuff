namespace ClickView.GoodStuff.Hosting.Queue;

using Queues.RabbitMq;

public abstract class BaseQueueMessage<TData>
{
    protected readonly MessageContext<TData> MessageContext;

    internal BaseQueueMessage(MessageContext<TData> messageContext)
    {
        MessageContext = messageContext;
    }

    /// <summary>
    /// The message data
    /// </summary>
    public TData Data => MessageContext.Data;

    /// <summary>
    /// The routing key of the message. If not set, this will be an empty string.
    /// </summary>
    public string RoutingKey => MessageContext.RoutingKey;

    /// <summary>
    /// The timestamp the message was enqueued
    /// </summary>
    public DateTime Timestamp => MessageContext.Timestamp;

    /// <summary>
    /// The unique ID for this message
    /// </summary>
    public string Id => MessageContext.Id;

    /// <summary>
    /// The priority of the message
    /// </summary>
    public MessagePriority Priority => MessageContext.Priority;

    /// <summary>
    /// True if the message has been re-delivered
    /// </summary>
    public bool ReDelivered => MessageContext.ReDelivered;
}
