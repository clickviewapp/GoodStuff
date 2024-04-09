namespace ClickView.GoodStuff.Queues.RabbitMq;

using System.Security.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
    public IMessageSerializer Serializer { get; set; } = NewtonsoftJsonMessageSerializer.Default;

    /// <summary>
    /// The logger factory to enable logging
    /// </summary>
    public ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;

    public RabbitMqClientOptions Value => this;
}
