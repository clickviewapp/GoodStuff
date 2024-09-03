namespace ClickView.GoodStuff.Hosting.Queue;

/// <summary>
/// Options for the <see cref="QueueHostedService{TMessage,TOptions}"/>
/// </summary>
public abstract class BatchQueueHostedServiceOptions : BaseQueueHostedServiceOptions
{
    /// <summary>
    /// The number messages to keep in memory before batching.
    /// </summary>
    public ushort BatchSize { get; set; } = 100;

    /// <summary>
    /// The minimum time between messages that processing will occur.
    ///
    /// For example, if this value is set to 5 seconds and only one message is received,
    /// then after 5 seconds of receiving the message the batch process will run.
    /// </summary>
    public TimeSpan MinFlushInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// The maximum time a message can sit in the buffer before being processed.
    /// </summary>
    public TimeSpan MaxFlushInterval { get; set; } = TimeSpan.FromMinutes(1);
}
