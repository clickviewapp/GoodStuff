namespace ClickView.GoodStuff.Queues.RabbitMq;

using System.Security.Authentication;
using Microsoft.Extensions.Options;
using Serialization;

public class RabbitMqClientOptions : IOptions<RabbitMqClientOptions>
{
    public required string Host { get; set; }
    public int Port { get; set; } = 5672;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public TimeSpan? ConnectionTimeout { get; set; }
    public IMessageSerializer Serializer { get; set; } = NewtonsoftJsonMessageSerializer.Default;
    public SslProtocols? SslProtocols { get; set; }

    public RabbitMqClientOptions Value => this;
}
