namespace ClickView.GoodStuff.Queues.RabbitMq;

using Internal;
using RabbitMQ.Client;

public class SubscriptionContext : IAsyncDisposable
{
    private readonly IModel _channel;
    private readonly ActiveSubscriptions _subscriptions;
    private string? _consumerTag;
    private bool _disposed;

    internal SubscriptionContext(IModel channel, ActiveSubscriptions subscriptions)
    {
        _channel = channel;
        _subscriptions = subscriptions;
    }

    internal void SetConsumerTag(string consumerTag) => _consumerTag = consumerTag;

    public Task AcknowledgeAsync(ulong deliveryTag, bool multiple = false)
    {
        CheckDisposed();

        _channel.BasicAck(
            deliveryTag: deliveryTag,
            multiple: multiple);

        return Task.CompletedTask;
    }

    public Task NegativeAcknowledgeAsync(ulong deliveryTag, bool multiple = false, bool requeue = true)
    {
        CheckDisposed();

        _channel.BasicNack(
            deliveryTag: deliveryTag,
            multiple: multiple,
            requeue: requeue);

        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        if (_disposed)
            return ValueTask.CompletedTask;

        // Cancel first
        _channel.BasicCancel(_consumerTag);
        _channel.Close();

        _disposed = true;
        _channel.Dispose();

        _subscriptions.Remove(this);

        return ValueTask.CompletedTask;
    }

    private void CheckDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().Name);
    }
}
