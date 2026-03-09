namespace ClickView.GoodStuff.Queues.RabbitMq;

using Serialization;

/// <summary>
/// Additional options for queue subscriptions.
/// </summary>
public class SubscribeOptions
{
    /// <summary>
    /// Set to true to automatically acknowledge messages when they are received.
    /// </summary>
    public bool AutoAcknowledge { get; init; }

    /// <summary>
    /// The maximum number of unacknowledged messages that RabbitMQ can deliver to this consumer at once.
    /// </summary>
    public ushort PrefetchCount { get; init; } = 1;

    /// <summary>
    /// Set to a value greater than one to enable concurrent processing. For a concurrency greater than one,
    /// tasks will be offloaded to the worker thread pool so it is important to choose the value for the concurrency wisely to avoid thread pool overloading.
    /// If set to null, the value from <see cref="RabbitMqClientOptions"/> will be used.
    /// </summary>
    /// <remarks>For concurrency greater than one this removes the guarantee that consumers handle messages in the order they receive them.
    /// In addition to that consumers need to be thread/concurrency safe.</remarks>
    public ushort? ConsumerDispatchConcurrency { get; set; }

    /// <summary>
    /// Optional serializer override for this subscription.
    /// If null, <see cref="RabbitMqClientOptions.Serializer"/> is used.
    /// </summary>
    public IMessageSerializer? Serializer { get; set; }

    internal static readonly SubscribeOptions Default = new();
}
