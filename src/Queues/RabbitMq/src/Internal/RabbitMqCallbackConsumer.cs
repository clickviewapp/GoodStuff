namespace ClickView.GoodStuff.Queues.RabbitMq.Internal;

using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Serialization;

internal class RabbitMqCallbackConsumer<TData> : AsyncDefaultBasicConsumer
{
    private readonly SubscriptionContext _subscriptionContext;
    private readonly Func<MessageContext<TData>, CancellationToken, Task> _callback;
    private readonly CountWaiter _taskWaiter;
    private readonly IMessageSerializer _serializer;
    private readonly ILogger<RabbitMqCallbackConsumer<TData>> _logger;

    public RabbitMqCallbackConsumer(SubscriptionContext subscriptionContext,
        Func<MessageContext<TData>, CancellationToken, Task> callback,
        CountWaiter taskWaiter,
        IMessageSerializer serializer,
        ILogger<RabbitMqCallbackConsumer<TData>> logger)
    {
        _subscriptionContext = subscriptionContext;
        _callback = callback;
        _taskWaiter = taskWaiter;
        _serializer = serializer;
        _logger = logger;
    }

    public override Task HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered,
        string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
    {
        _logger.QueueMessageReceived(deliveryTag, consumerTag, exchange, redelivered);

        return HandleBasicDeliverAsync(deliveryTag, body);
    }

    private async Task HandleBasicDeliverAsync(ulong deliveryTag, ReadOnlyMemory<byte> body)
    {
        try
        {
            var message = _serializer.Deserialize<TData>(body.Span);

            if (message is null)
                throw new RabbitMqClientException("Failed to deserialize message");

            var context = new MessageContext<TData>(
                data: message.Data,
                deliveryTag: deliveryTag,
                timestamp: DateTimeOffset.FromUnixTimeSeconds(message.Timestamp).UtcDateTime,
                id: message.Id,
                subscriptionContext: _subscriptionContext
            );

            _taskWaiter.Increment();
            try
            {
                await _callback(context, CancellationToken.None);
            }
            finally
            {
                _taskWaiter.Decrement();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception when processing queue message");
            throw;
        }
    }
}
