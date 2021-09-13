namespace ClickView.GoodStuff.AspNetCore.Authentication.StackExchangeRedis
{
    using Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class ServiceCollectionExtensions
    {
        public static void AddRedisCacheUserSessionStore(this IServiceCollection services,
            Action<RedisUserSessionCacheOptions> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.AddOptions<RedisUserSessionCacheOptions>()
                .Validate(o =>
                {
                    o.Validate();
                    return true;
                })
                .Configure(configure);

            //redis user session cache
            services.AddSingleton<IUserSessionStore, RedisUserSessionStore>();
        }
    }
}
