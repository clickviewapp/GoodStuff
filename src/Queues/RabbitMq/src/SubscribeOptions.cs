namespace ClickView.GoodStuff.Queues.RabbitMq;

public class SubscribeOptions
{
    public bool AutoAcknowledge { get; init; }
    public ushort PrefetchCount { get; init; } = 1;

    internal static readonly SubscribeOptions Default = new();
}
