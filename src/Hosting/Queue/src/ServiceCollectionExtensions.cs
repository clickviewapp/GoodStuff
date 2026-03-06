namespace ClickView.GoodStuff.Hosting.Queue;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <param name="services"></param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Add a <see cref="QueueHostedService{TMessage,TOptions}"/> registration for the given type.
        /// </summary>
        /// <param name="config"></param>
        /// <typeparam name="THostedService"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TOptions"></typeparam>
        public void AddQueueHostedService<THostedService, TMessage, TOptions>(IConfiguration config)
            where THostedService : QueueHostedService<TMessage, TOptions>
            where TMessage : class, new()
            where TOptions : QueueHostedServiceOptions
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(config);

            services.Configure<TOptions>(config);
            services.AddQueueHostedService<THostedService, TMessage, TOptions>();
        }

        /// <summary>
        /// Add a <see cref="QueueHostedService{TMessage,TOptions}"/> registration for the given type.
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="THostedService"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TOptions"></typeparam>
        public void AddQueueHostedService<THostedService, TMessage, TOptions>(Action<TOptions> configure)
            where THostedService : QueueHostedService<TMessage, TOptions>
            where TMessage : class, new()
            where TOptions : QueueHostedServiceOptions
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configure);

            services.Configure(configure);
            services.AddQueueHostedService<THostedService, TMessage, TOptions>();
        }

        /// <summary>
        /// Add a <see cref="QueueHostedService{TMessage,TOptions}"/> registration for the given type.
        /// </summary>
        /// <typeparam name="THostedService"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TOptions"></typeparam>
        public void AddQueueHostedService<THostedService, TMessage, TOptions>()
            where THostedService : QueueHostedService<TMessage, TOptions>
            where TMessage : class, new()
            where TOptions : QueueHostedServiceOptions
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddHostedService<THostedService>();
        }
    }
}
