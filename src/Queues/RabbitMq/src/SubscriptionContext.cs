namespace ClickView.GoodStuff.Queues.RabbitMq;

using System.Diagnostics;
using Internal;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

public class SubscriptionContext : IAsyncDisposable
{
    private readonly string _queueName;
    private readonly IChannel _channel;
    private readonly ActiveSubscriptions _subscriptions;
    private readonly CountWaiter _taskWaiter;
    private readonly ILogger<SubscriptionContext>? _logger;
    private string? _consumerTag;
    private bool _disposed;

    internal SubscriptionContext(
        string queueName,
        IChannel channel,
        ActiveSubscriptions subscriptions,
        CountWaiter taskWaiter,
        ILogger<SubscriptionContext>? logger)
    {
        _queueName = queueName;
        _channel = channel;
        _subscriptions = subscriptions;
        _taskWaiter = taskWaiter;
        _logger = logger;
    }

    internal void SetConsumerTag(string consumerTag) => _consumerTag = consumerTag;

    /// <summary>
    /// Returns true if the RabbitMQ channel is open
    /// </summary>
    public bool IsOpen => _channel.IsOpen;

    /// <summary>
    /// Acknowledges one or more messages
    /// </summary>
    /// <param name="deliveryTag">The delivery tag of the message to acknowledge</param>
    /// <param name="multiple">If true, acknowledge all outstanding delivery tags up to and including the delivery tag</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public ValueTask AcknowledgeAsync(ulong deliveryTag, bool multiple = false,
        CancellationToken cancellationToken = default)
    {
        CheckDisposed();

        _logger?.SendingAcknowledge(deliveryTag);

        return _channel.BasicAckAsync(
            deliveryTag: deliveryTag,
            multiple: multiple,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Rejects one or more messages
    /// </summary>
    /// <param name="deliveryTag">The delivery tag of the message to acknowledge</param>
    /// <param name="multiple">If true, reject all outstanding delivery tags up to and including the delivery tag</param>
    /// <param name="requeue">If true, requeue the delivery (or multiple deliveries if <paramref name="multiple"/> is true)) with the specified delivery tag</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public ValueTask NegativeAcknowledgeAsync(ulong deliveryTag, bool multiple = false, bool requeue = true,
        CancellationToken cancellationToken = default)
    {
        CheckDisposed();

        _logger?.SendingNegativeAcknowledge(deliveryTag);

        return _channel.BasicNackAsync(
            deliveryTag: deliveryTag,
            multiple: multiple,
            requeue: requeue,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _logger?.LogDebug("Disposing subscription context...");

        // Unsubscribe from the queue to stop receiving any new messages
        _logger?.LogDebug("Unsubscribing from queue {QueueName}", _queueName);
        Debug.Assert(_consumerTag != null);
        await _channel.BasicCancelAsync(_consumerTag);

        // Wait for all tasks to complete before closing the connection
        // If we close the connection first then the tasks cant ack
        _logger?.LogDebug("Waiting for all tasks to finish...");
        await _taskWaiter.WaitAsync();

        // Close the channel
        _logger?.LogDebug("Closing channel");
        await _channel.CloseAsync();

        // Dispose
        _disposed = true;
        _channel.Dispose();

        _subscriptions.Remove(this);

        _logger?.LogDebug("Subscription context disposed");
    }

    private void CheckDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
