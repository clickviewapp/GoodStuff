namespace ClickView.GoodStuff.AspNetCore.Authentication
{
    using Abstractions;
    using Endpoints;
    using Infrastructure;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;
    using StackExchangeRedis;
    using System;
    using TokenValidation;

    public static class ServiceCollectionExtensions
    {
        public static void AddOpenIdConnectSessionHandler(this IServiceCollection services, Action<OpenIdConnectSessionHandlerOptions> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.AddOptions<OpenIdConnectSessionHandlerOptions>()
                .Validate(o =>
                {
                    o.Validate();
                    return true;
                })
                .Configure(configure);

            //Add post configure options for cookies to add the ticket store
            services.TryAddEnumerable(ServiceDescriptor
                .Singleton<IPostConfigureOptions<CookieAuthenticationOptions>, PostCookieAuthenticationOptions>());

            // Endpoints
            services.AddSingleton<BackChannelLogoutEndpoint>();

            // Stores
            services.AddSingleton<IDataSerializer<AuthenticationTicket>, TicketSerializer>();
            services.AddSingleton<IUserSessionTicketStore, UserSessionTicketStore>();

            // Token Validation
            services.AddOptions<TokenValidatorOptions>();
            services.AddSingleton<ITokenValidator, TokenValidator>();

            services.TryAddEnumerable(ServiceDescriptor
                .Singleton<IPostConfigureOptions<TokenValidatorOptions>, TokenValidatorOptionsConfigureOptions>());
        }

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
