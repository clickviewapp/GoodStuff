namespace ClickView.GoodStuff.Queues.RabbitMq.Internal;

using RabbitMQ.Client;
using Serialization;

internal class RabbitMqCallbackConsumer<TData> : AsyncDefaultBasicConsumer
{
    private readonly SubscriptionContext _subscriptionContext;
    private readonly Func<MessageContext<TData>, CancellationToken, Task> _callback;
    private readonly IMessageSerializer _serializer;

    public RabbitMqCallbackConsumer(SubscriptionContext subscriptionContext,
        Func<MessageContext<TData>, CancellationToken, Task> callback,
        IMessageSerializer serializer)
    {
        _subscriptionContext = subscriptionContext;
        _callback = callback;
        _serializer = serializer;
    }

    public override async Task HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered,
        string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
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

        await _callback(context, CancellationToken.None);
    }
}
