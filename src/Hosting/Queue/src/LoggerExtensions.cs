namespace ClickView.GoodStuff.Hosting.Queue;

using Microsoft.Extensions.Logging;

internal static partial class LoggerExtensions
{
    [LoggerMessage(1, LogLevel.Information, "Queue service started - {Name}", EventName = "QueueServiceStarted")]
    public static partial void QueueServiceStarted(this ILogger logger, string name);

    [LoggerMessage(2, LogLevel.Information, "Queue service stopped - {Name}", EventName = "QueueServiceStopped")]
    public static partial void QueueServiceStopped(this ILogger logger, string name);

    [LoggerMessage(3, LogLevel.Debug, "Queue message received", EventName = "QueueMessageReceived")]
    public static partial void QueueMessageReceived(this ILogger logger);

    [LoggerMessage(4, LogLevel.Warning, "Cannot acknowledge task {DeliveryTag}. Channel is not open",
        EventName = "AcknowledgeFailureChannelNotOpen")]
    public static partial void AcknowledgeFailureChannelNotOpen(this ILogger logger, ulong deliveryTag);

    [LoggerMessage(5, LogLevel.Information, "Message buffer limit reached ({MessageBufferSize}). Processing items",
        EventName = "BatchProcessMessageBufferReached")]
    public static partial void BatchProcessMessageBufferReached(this ILogger logger, ushort messageBufferSize);

    [LoggerMessage(6, LogLevel.Information, "Last process occured at {LastProcessTime}. Processing items",
        EventName = "BatchProcessIntervalReached")]
    public static partial void BatchProcessIntervalReached(this ILogger logger, DateTime lastProcessTime);

    [LoggerMessage(7, LogLevel.Debug, "Max flush interval reached ({TimeSinceLastProcess})",
        EventName = "BatchProcessMaxFlushIntervalReached")]
    public static partial void BatchProcessMaxFlushIntervalReached(this ILogger logger, TimeSpan timeSinceLastProcess);

    [LoggerMessage(8, LogLevel.Debug, "Last message received {TimeSinceLastMessage} ago. Waiting {WaitTime} until next flush interval",
        EventName = "BatchProcessMinFlushIntervalNotReached")]
    public static partial void BatchProcessMinFlushIntervalNotReached(this ILogger logger, TimeSpan timeSinceLastMessage, TimeSpan waitTime);

    [LoggerMessage(9, LogLevel.Debug, "Waiting {WaitTime} until next flush interval",
        EventName = "BatchProcessIntervalWait")]
    public static partial void BatchProcessIntervalWait(this ILogger logger, TimeSpan waitTime);

    [LoggerMessage(10, LogLevel.Information, "Processing {Count} items...", EventName = "BatchProcessStart")]
    public static partial void BatchProcessStart(this ILogger logger, int count);

    [LoggerMessage(11, LogLevel.Information, "Processed {Count} items in {ProcessDuration}",
        EventName = "BatchProcessCompleted")]
    public static partial void BatchProcessCompleted(this ILogger logger, int count, TimeSpan processDuration);
}