namespace ClickView.GoodStuff.Queues.RabbitMq.Internal;

using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Serialization;

internal class RabbitMqCallbackConsumer<TData>(
    IChannel channel,
    SubscriptionContext subscriptionContext,
    Func<MessageContext<TData>, CancellationToken, Task> callback,
    CountWaiter taskWaiter,
    IMessageSerializer serializer,
    ILogger<RabbitMqCallbackConsumer<TData>>? logger)
    : AsyncDefaultBasicConsumer(channel)
{
    public override Task HandleBasicDeliverAsync(string consumerTag, ulong deliveryTag, bool redelivered,
        string exchange, string routingKey, IReadOnlyBasicProperties properties, ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
    {
        logger?.QueueMessageReceived(deliveryTag, consumerTag, exchange, redelivered);

        return HandleBasicDeliverAsync(deliveryTag, body, properties, routingKey, redelivered, cancellationToken);
    }

    private async Task HandleBasicDeliverAsync(ulong deliveryTag, ReadOnlyMemory<byte> body,
        IReadOnlyBasicProperties properties, string routingKey, bool reDelivered, CancellationToken cancellationToken)
    {
        try
        {
            var message = serializer.Deserialize<TData>(body.Span);

            if (message is null)
                throw new RabbitMqClientException("Failed to deserialize message");

            // If we have a delivery count, get it
            long? deliveryCount = null;
            if (properties.TryGetHeaderValue<long>("x-delivery-count", out var deliveryCountValue))
                deliveryCount = deliveryCountValue;

            var context = new MessageContext<TData>(
                data: message.Data,
                routingKey: routingKey,
                deliveryTag: deliveryTag,
                timestamp: DateTimeOffset.FromUnixTimeSeconds(message.Timestamp).UtcDateTime,
                id: message.Id,
                priority: new MessagePriority(properties.Priority),
                reDelivered: reDelivered,
                subscriptionContext: subscriptionContext,
                deliveryCount: deliveryCount
            );

            taskWaiter.Increment();
            try
            {
                await callback(context, cancellationToken);
            }
            finally
            {
                taskWaiter.Decrement();
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Unhandled exception when processing queue message");
            throw;
        }
    }
}
