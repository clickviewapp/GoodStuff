namespace ClickView.GoodStuff.Queues.RabbitMq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public QueueClientBuilder AddKeyedRabbitMq(string name, Action<RabbitMqClientOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.Configure(name, configure);
            services.AddSingleton<IPostConfigureOptions<RabbitMqClientOptions>, PostRabbitMqOptions>();
            services.AddKeyedSingleton<IQueueClient>(name, (provider, o) =>
            {
                var optionsName = (string) o!;
                var options = provider.GetRequiredService<IOptionsMonitor<RabbitMqClientOptions>>().Get(optionsName);
                return new RabbitMqClient(options);
            });

            return new QueueClientBuilder(services, name);
        }

        public QueueClientBuilder AddRabbitMq(Action<RabbitMqClientOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.Configure(configure);
            services.AddSingleton<IPostConfigureOptions<RabbitMqClientOptions>, PostRabbitMqOptions>();
            services.AddSingleton<IQueueClient, RabbitMqClient>();

            return new QueueClientBuilder(services);
        }
    }
}
