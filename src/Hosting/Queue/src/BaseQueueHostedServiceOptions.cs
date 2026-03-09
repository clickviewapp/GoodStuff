namespace ClickView.GoodStuff.Hosting.Queue;

using Queues.RabbitMq.Serialization;

/// <summary>
/// Base queue options which are shared amongst the various hosted queue workers
/// </summary>
public abstract class BaseQueueHostedServiceOptions
{
    /// <summary>
    /// The name of the queue to subscribe to.
    /// </summary>
    public string? QueueName { get; set; }

    public IMessageSerializer? Serializer { get; set; }
}
