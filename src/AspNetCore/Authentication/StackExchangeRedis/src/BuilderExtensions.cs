namespace ClickView.GoodStuff.AspNetCore.Authentication.StackExchangeRedis
{
    using Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class BuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="IUserSessionStore"/> which stores data using StackExchange Redis
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static OpenIdConnectSessionHandlerBuilder AddStackExchangeRedisUserSessionStore(
            this OpenIdConnectSessionHandlerBuilder builder,
            Action<RedisUserSessionCacheOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(configure);

            builder.Services.AddOptions<RedisUserSessionCacheOptions>()
                .Validate(o =>
                {
                    o.Validate();
                    return true;
                })
                .Configure(configure);

            //redis user session cache
            builder.Services.AddSingleton<IUserSessionStore, RedisUserSessionStore>();

            return builder;
        }
    }
}
