namespace ClickView.GoodStuff.Queues.RabbitMq;

public class SubscribeOptions
{
    public bool AutoAcknowledge { get; set; }
    public ushort PrefetchCount { get; set; } = 1;

    internal static readonly SubscribeOptions Default = new();
}
