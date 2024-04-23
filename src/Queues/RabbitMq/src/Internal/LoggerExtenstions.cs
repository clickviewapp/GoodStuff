namespace ClickView.GoodStuff.Queues.RabbitMq.Internal;

using Microsoft.Extensions.Logging;

internal static partial class LoggerExtensions
{
    [LoggerMessage(1, LogLevel.Debug, "Sending Acknowledge {DeliveryTag}", EventName = "SendingAcknowledge")]
    public static partial void SendingAcknowledge(this ILogger logger, ulong deliveryTag);

    [LoggerMessage(2, LogLevel.Debug, "Sending NegativeAcknowledge {DeliveryTag}", EventName = "SendingNegativeAcknowledge")]
    public static partial void SendingNegativeAcknowledge(this ILogger logger, ulong deliveryTag);

    [LoggerMessage(3, LogLevel.Information, "Successfully subscribed to queue {QueueName}", EventName = "SubscribedToQueue")]
    public static partial void SubscribedToQueue(this ILogger logger, string queueName);

    [LoggerMessage(4, LogLevel.Debug, "Queue message received. DeliveryTag: {DeliveryTag}, ConsumerTag: {ConsumerTag}, Exchange: {Exchange}, Redelivered: {Redelivered}", EventName = "QueueMessageReceived")]
    public static partial void QueueMessageReceived(this ILogger logger, ulong deliveryTag, string consumerTag, string exchange, bool redelivered);

    [LoggerMessage(5, LogLevel.Debug, "Unsubscribing {Count} listeners", EventName = "UnsubscribingListeners")]
    public static partial void UnsubscribingListeners(this ILogger logger, int count);

    [LoggerMessage(6, LogLevel.Debug, "Connecting to RabbitMQ...", EventName = "ConnectingToRabbitMQ")]
    public static partial void ConnectingToRabbitMq(this ILogger logger);

    [LoggerMessage(7, LogLevel.Debug, "Connection to RabbitMQ successful", EventName = "ConnectedToRabbitMQ")]
    public static partial void ConnectedToRabbitMq(this ILogger logger);
}
