namespace ClickView.GoodStuff.Queues.RabbitMq;

public class EnqueueOptions
{
    public bool Persistent { get; init; }
    public string RoutingKey { get; init; } = string.Empty;

    /// <summary>
    /// If true, enables publisher confirm tracking.
    /// See <see href="https://www.rabbitmq.com/docs/confirms#publisher-confirms"/> for more information.
    /// </summary>
    public bool EnablePublisherConfirms { get; set; }

    internal static readonly EnqueueOptions Default = new();
}
