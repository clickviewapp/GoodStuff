namespace ClickView.GoodStuff.Queues.RabbitMq.Internal;

using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Serialization;

internal class RabbitMqCallbackConsumer<TData>(
    SubscriptionContext subscriptionContext,
    Func<MessageContext<TData>, CancellationToken, Task> callback,
    CountWaiter taskWaiter,
    IMessageSerializer serializer,
    ILogger<RabbitMqCallbackConsumer<TData>> logger)
    : AsyncDefaultBasicConsumer
{
    public override Task HandleBasicDeliverAsync(string consumerTag, ulong deliveryTag, bool redelivered,
        string exchange, string routingKey, IReadOnlyBasicProperties properties, ReadOnlyMemory<byte> body)
    {
        logger.QueueMessageReceived(deliveryTag, consumerTag, exchange, redelivered);

        return HandleBasicDeliverAsync(deliveryTag, body);
    }

    private async Task HandleBasicDeliverAsync(ulong deliveryTag, ReadOnlyMemory<byte> body)
    {
        try
        {
            var message = serializer.Deserialize<TData>(body.Span);

            if (message is null)
                throw new RabbitMqClientException("Failed to deserialize message");

            var context = new MessageContext<TData>(
                data: message.Data,
                deliveryTag: deliveryTag,
                timestamp: DateTimeOffset.FromUnixTimeSeconds(message.Timestamp).UtcDateTime,
                id: message.Id,
                subscriptionContext: subscriptionContext
            );

            taskWaiter.Increment();
            try
            {
                await callback(context, CancellationToken.None);
            }
            finally
            {
                taskWaiter.Decrement();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception when processing queue message");
            throw;
        }
    }
}
