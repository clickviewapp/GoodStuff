namespace ClickView.GoodStuff.Queues.RabbitMq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

/// <summary>
/// Dependency injection extensions for registering <see cref="IQueueClient"/> backed by RabbitMQ.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds a keyed RabbitMQ queue client registration.
        /// </summary>
        /// <param name="name">The service key and options name for the client.</param>
        /// <param name="configure">The options configuration callback.</param>
        /// <returns>A builder for further RabbitMQ options configuration.</returns>
        public RabbitMqClientBuilder AddKeyedRabbitMq(string name, Action<RabbitMqClientOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.Configure(name, configure);
            services.AddKeyedSingleton<IQueueClient>(name, KeyedClientBuilder);

            return services.AddRabbitMqCore(name);
        }

        /// <summary>
        /// Adds the default (non-keyed) RabbitMQ queue client registration.
        /// </summary>
        /// <param name="configure">The options configuration callback.</param>
        /// <returns>A builder for further RabbitMQ options configuration.</returns>
        public RabbitMqClientBuilder AddRabbitMq(Action<RabbitMqClientOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.Configure(configure);
            services.AddSingleton<IQueueClient, RabbitMqClient>();

            return services.AddRabbitMqCore(string.Empty);
        }

        private RabbitMqClientBuilder AddRabbitMqCore(string name)
        {
            // Setup post configure hook to configure logging
            services.TryAddSingleton<IPostConfigureOptions<RabbitMqClientOptions>, PostRabbitMqOptions>();

            return new RabbitMqClientBuilder(services, name);
        }

        private static RabbitMqClient KeyedClientBuilder(IServiceProvider provider, object? keyName)
        {
            var optionsName = (string) keyName!;
            var options = provider.GetRequiredService<IOptionsMonitor<RabbitMqClientOptions>>().Get(optionsName);
            return new RabbitMqClient(options);
        }
    }
}
