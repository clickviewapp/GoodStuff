namespace ClickView.GoodStuff.Queues.RabbitMq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public RabbitMqClientBuilder AddKeyedRabbitMq(string name, Action<RabbitMqClientOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.Configure(name, configure);
            services.AddKeyedSingleton<IQueueClient>(name, KeyedClientBuilder);

            return services.AddRabbitMqCore(name);
        }

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
