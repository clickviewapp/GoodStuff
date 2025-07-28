namespace ClickView.GoodStuff.Queues.RabbitMq;

public class SubscribeOptions
{
    public bool AutoAcknowledge { get; init; }
    public ushort PrefetchCount { get; init; } = 1;

    /// <summary>
    /// Set to a value greater than one to enable concurrent processing. For a concurrency greater than one,
    /// tasks will be offloaded to the worker thread pool so it is important to choose the value for the concurrency wisely to avoid thread pool overloading.
    /// If set to null, the value from <see cref="RabbitMqClientOptions"/> will be used.
    /// </summary>
    /// <remarks>For concurrency greater than one this removes the guarantee that consumers handle messages in the order they receive them.
    /// In addition to that consumers need to be thread/concurrency safe.</remarks>
    public ushort? ConsumerDispatchConcurrency { get; set; }

    internal static readonly SubscribeOptions Default = new();
}
