namespace ClickView.GoodStuff.AspNetCore.Authentication;

using Endpoints;
using Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using TokenValidation;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds OpenIdConnection session handling to the specified <see cref="IServiceCollection" />
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static OpenIdConnectSessionHandlerBuilder AddOpenIdConnectSessionHandler(
        this IServiceCollection services,
        Action<OpenIdConnectSessionHandlerOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

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

        return new OpenIdConnectSessionHandlerBuilder(services);
    }
}
