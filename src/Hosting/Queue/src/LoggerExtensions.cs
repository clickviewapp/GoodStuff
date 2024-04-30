namespace ClickView.GoodStuff.Hosting.Queue;

using Microsoft.Extensions.Logging;

internal static partial class LoggerExtensions
{
    [LoggerMessage(1, LogLevel.Information, "Queue service started - {Name}")]
    public static partial void QueueServiceStarted(this ILogger logger, string name);

    [LoggerMessage(2, LogLevel.Information, "Queue service stopped - {Name}")]
    public static partial void QueueServiceStopped(this ILogger logger, string name);

    [LoggerMessage(3, LogLevel.Debug, "Queue message received (Id: {Id}, Timestamp: {Timestamp})")]
    public static partial void QueueMessageReceived(this ILogger logger, string id, DateTime timestamp);

    [LoggerMessage(4, LogLevel.Warning, "Cannot acknowledge task {DeliveryTag}. Channel is not open")]
    public static partial void AcknowledgeFailureChannelNotOpen(this ILogger logger, ulong deliveryTag);

    [LoggerMessage(5, LogLevel.Debug, "Last process occured at {LastProcessTime}. (Time since last process: {TimeSinceLastProcess})")]
    public static partial void LogLastProcessTime(this ILogger logger, DateTime lastProcessTime, TimeSpan timeSinceLastProcess);

    [LoggerMessage(6, LogLevel.Debug, "Waiting {WaitTime} until next flush interval. (Time since last message: {TimeSinceLastMessage})")]
    public static partial void BatchProcessIntervalWait(this ILogger logger, TimeSpan waitTime, TimeSpan timeSinceLastMessage);

    [LoggerMessage(7, LogLevel.Information, "Processing {Count} items (Reason: {Reason})...")]
    public static partial void BatchProcessStart(this ILogger logger, int count, FlushReason reason);

    [LoggerMessage(8, LogLevel.Information, "Processed {Count} items in {ProcessDuration}")]
    public static partial void BatchProcessCompleted(this ILogger logger, int count, TimeSpan processDuration);
}
