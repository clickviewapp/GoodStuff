namespace ClickView.GoodStuff.Queues.RabbitMq;

using System.Security.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serialization;

public class RabbitMqClientOptions : IOptions<RabbitMqClientOptions>
{
    /// <summary>
    /// The hostname of the RabbitMQ host
    /// </summary>
    public string Host { get; set; } = null!;

    /// <summary>
    /// The port of the RabbitMQ instance
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// Username for authenticating with RabbitMQ
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Password for authenticating with RabbitMQ
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
    public SslProtocols SslVersion { get; set; } = SslProtocols.None;

    /// <summary>
    /// The serializer to use for serializing and deserializing messages
    /// </summary>
    public IMessageSerializer Serializer { get; set; } = new NewtonsoftJsonMessageSerializer();

    /// <summary>
    /// The logger factory to enable logging
    /// </summary>
    public ILoggerFactory? LoggerFactory { get; set; }

    /// <summary>
    /// Set to a value greater than one to enable concurrent processing. For a concurrency greater than one,
    /// tasks will be offloaded to the worker thread pool so it is important to choose the value for the concurrency wisely to avoid thread pool overloading.
    /// Defaults to 1.
    /// </summary>
    /// <remarks>For concurrency greater than one this removes the guarantee that consumers handle messages in the order they receive them.
    /// In addition to that consumers need to be thread/concurrency safe.</remarks>
    public ushort ConsumerDispatchConcurrency { get; set; } = 1;

    public RabbitMqClientOptions Value => this;
}
