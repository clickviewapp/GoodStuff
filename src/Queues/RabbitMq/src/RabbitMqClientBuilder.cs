namespace ClickView.GoodStuff.Queues.RabbitMq;

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serialization;

/// <summary>
/// Builder for configuring <see cref="RabbitMqClientOptions"/> registered in dependency injection.
/// </summary>
public class RabbitMqClientBuilder
{
    private readonly string _name;

    internal RabbitMqClientBuilder(IServiceCollection services, string name)
    {
        _name = name;
        Services = services;
    }

    /// <summary>
    /// The <see cref="IServiceCollection"/> being configured.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Configures Newtonsoft.Json as the serializer for this client registration.
    /// </summary>
    /// <param name="configure">Optional settings callback.</param>
    /// <returns>The current builder.</returns>
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

    /// <summary>
    /// Configures System.Text.Json as the serializer for this client registration.
    /// </summary>
    /// <param name="configure">Optional options callback.</param>
    /// <returns>The current builder.</returns>
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
