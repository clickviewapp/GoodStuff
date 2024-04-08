namespace ClickView.GoodStuff.Queues.RabbitMq;

public class EnqueueOptions
{
    public bool Persistent { get; init; }
    public string? RoutingKey { get; init; }

    internal static readonly EnqueueOptions Default = new();
}
