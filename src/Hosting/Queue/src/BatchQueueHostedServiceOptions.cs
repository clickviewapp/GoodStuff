namespace ClickView.GoodStuff.Hosting.Queue;

/// <summary>
/// Options for the <see cref="QueueHostedService{TMessage,TOptions}"/>
/// </summary>
public abstract class BatchQueueHostedServiceOptions : BaseQueueHostedServiceOptions
{
    /// <summary>
    /// The number of tasks to run concurrently.
    /// </summary>
    public ushort BatchSize { get; set; } = 100;

    public TimeSpan MinFlushInterval { get; set; } = TimeSpan.FromSeconds(5);

    public TimeSpan MaxFlushInterval { get; set; } = TimeSpan.FromMinutes(1);
}
