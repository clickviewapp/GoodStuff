namespace ClickView.GoodStuff.Hosting.Queue;

/// <summary>
/// Options for the <see cref="QueueHostedService{TMessage,TOptions}"/>
/// </summary>
public abstract class QueueHostedServiceOptions : BaseQueueHostedServiceOptions
{
    /// <summary>
    /// The number of tasks to run concurrently.
    /// </summary>
    public ushort ConcurrentTaskCount { get; set; } = 1;
}
