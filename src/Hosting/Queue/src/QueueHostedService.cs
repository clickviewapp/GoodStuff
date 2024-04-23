namespace ClickView.GoodStuff.Hosting.Queue;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Queues.RabbitMq;

/// <summary>
/// A hosted service that connects to a queue and executes <see cref="OnMessageAsync(TMessage,System.Threading.CancellationToken)" />
/// when a message is received
/// </summary>
/// <typeparam name="TMessage"></typeparam>
/// <typeparam name="TOptions"></typeparam>
public abstract class QueueHostedService<TMessage, TOptions> : BaseQueueHostedService<TOptions>
    where TOptions : QueueHostedServiceOptions
{
    /// <summary>
    /// Initialises a new instance of <see cref="QueueHostedService{TMessage,TOptions}"/>.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="loggerFactory"></param>
    protected QueueHostedService(IOptions<TOptions> options, ILoggerFactory loggerFactory) : base(options, loggerFactory)
    {
    }

    /// <inheritdoc />
    protected override Task<SubscriptionContext> SubscribeAsync(IQueueClient queueClient, string queueName,
        CancellationToken cancellationToken)
    {
        return queueClient.SubscribeAsync<TMessage>(queueName,
            OnMessageAsync,
            new SubscribeOptions {PrefetchCount = Options.ConcurrentTaskCount},
            cancellationToken);
    }

    /// <summary>
    /// This method is called when a message has been received from the configured queue.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract Task OnMessageAsync(TMessage message, CancellationToken token);

    /// <summary>
    /// Triggered when a message is received from the configured queue.
    /// </summary>>
    /// <param name="messageContext"></param>
    /// <param name="cancellationToken"></param>
    private async Task OnMessageAsync(MessageContext<TMessage> messageContext, CancellationToken cancellationToken)
    {
        Logger.QueueMessageReceived(messageContext.Id, messageContext.Timestamp);

        try
        {
            await OnMessageAsync(messageContext.Data, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Propagate up to caller to handle
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unhandled exception caught when processing message");
        }

        await AcknowledgeAsync(messageContext.DeliveryTag, false, cancellationToken);
    }
}
