namespace ClickView.GoodStuff.Hosting.Queue;

using System.Security.Authentication;
using Queues.RabbitMq.Serialization;

/// <summary>
/// Options for the <see cref="QueueHostedService{TMessage,TOptions}"/>
/// </summary>
public abstract class QueueHostedServiceOptions
{
    /// <summary>
    /// The name of the queue to subscribe to.
    /// </summary>
    public string? QueueName { get; set; }

    /// <summary>
    /// The hostname or network address of the queue server in which to connect.
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// The port that the queue server is listening to.
    /// </summary>
    public ushort Port { get; set; } = 5672;

    /// <summary>
    /// The username of the user for the queue.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// The password of the user for the queue.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Timeout for connection attempts
    /// </summary>
    public TimeSpan? ConnectionTimeout { get; set; }

    /// <summary>
    /// Set to true to enable SSL
    /// </summary>
    public bool EnableSsl { get; set; }

    /// <summary>
    /// Set to false to ignore SSL errors
    /// </summary>
    public bool IgnoreSslErrors { get; set; }

    /// <summary>
    /// The TLS protocol version. Set to None to let the OS decide
    /// </summary>
    public SslProtocols? SslVersion { get; set; }

    /// <summary>
    /// The number of tasks to run concurrently.
    /// </summary>
    public ushort ConcurrentTaskCount { get; set; } = 1;

    /// <summary>
    /// The serializer to use for deserializing messages from the Queue
    /// </summary>
    public IMessageSerializer? Serializer { get; set; }
}
