namespace ClickView.GoodStuff.Queues.RabbitMq;

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serialization;

public class RabbitMqClientBuilder
{
    private readonly string _name;

    internal RabbitMqClientBuilder(IServiceCollection services, string name)
    {
        _name = name;
        Services = services;
    }

    public IServiceCollection Services { get; }

    public RabbitMqClientBuilder UseNewtonsoftJsonMessageSerializer(Action<JsonSerializerSettings>? configure = null)
    {
        Services.Configure<RabbitMqClientOptions>(_name, o =>
        {
            var settings = NewtonsoftJsonMessageSerializer.GetDefaultSettings();

            configure?.Invoke(settings);

            o.Serializer = new NewtonsoftJsonMessageSerializer(settings);
        });

        return this;
    }

    public RabbitMqClientBuilder UseSystemTextJsonSerializer(Action<JsonSerializerOptions>? configure = null)
    {
        Services.Configure<RabbitMqClientOptions>(_name, o =>
        {
            var options = SystemTextJsonMessageSerializer.GetDefaultOptions();

            configure?.Invoke(options);

            o.Serializer = new SystemTextJsonMessageSerializer(options);
        });

        return this;
    }
}
