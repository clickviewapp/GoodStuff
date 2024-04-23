namespace ClickView.GoodStuff.Hosting.Queue;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add a <see cref="QueueHostedService{TMessage,TOptions}"/> registration for the given type.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <typeparam name="THostedService"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TOptions"></typeparam>
    public static void AddQueueHostedService<THostedService, TMessage, TOptions>(this IServiceCollection services,
        IConfiguration config)
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
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <typeparam name="THostedService"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TOptions"></typeparam>
    public static void AddQueueHostedService<THostedService, TMessage, TOptions>(this IServiceCollection services,
        Action<TOptions> configure)
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
    /// <param name="services"></param>
    /// <typeparam name="THostedService"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TOptions"></typeparam>
    public static void AddQueueHostedService<THostedService, TMessage, TOptions>(this IServiceCollection services)
        where THostedService : QueueHostedService<TMessage, TOptions>
        where TMessage : class, new()
        where TOptions : QueueHostedServiceOptions
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<THostedService>();
    }
}
