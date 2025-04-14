namespace ClickView.GoodStuff.Hosting.Queue;

using Queues.RabbitMq;

public sealed class QueueMessage<TData>(MessageContext<TData> messageContext) : BaseQueueMessage<TData>(messageContext)
{
    /// <summary>
    /// Returns true if the message has been acknowledged (either ack or nack).
    /// </summary>
    public bool Acknowledged => MessageContext.Acknowledged;

    /// <summary>
    /// Acknowledges one or more messages
    /// </summary>
    /// <param name="multiple">If true, acknowledge all outstanding delivery tags up to and including the delivery tag</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public ValueTask AcknowledgeAsync(bool multiple = false, CancellationToken cancellationToken = default)
    {
        return MessageContext.AcknowledgeAsync(multiple, cancellationToken);
    }

    /// <summary>
    /// Rejects one or more messages
    /// </summary>
    /// <param name="multiple">If true, reject all outstanding delivery tags up to and including the delivery tag</param>
    /// <param name="requeue">If true, requeue the delivery (or multiple deliveries if <paramref name="multiple"/> is true)) with the specified delivery tag</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public ValueTask NegativeAcknowledgeAsync(bool multiple = false, bool requeue = true,
        CancellationToken cancellationToken = default)
    {
        return MessageContext.NegativeAcknowledgeAsync(multiple: multiple, requeue: requeue, cancellationToken);
    }
}
