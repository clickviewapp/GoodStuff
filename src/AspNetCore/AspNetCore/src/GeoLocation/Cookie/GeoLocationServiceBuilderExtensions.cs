namespace ClickView.GoodStuff.AspNetCore.GeoLocation.Cookie;

using System;
using Microsoft.Extensions.DependencyInjection;

public static class GeoLocationServiceBuilderExtensions
{
    public static GeoLocationServiceBuilder AddCookie(this GeoLocationServiceBuilder builder,
        Action<CookieGeoLocationProviderOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddSingleton<IGeoLocationProvider, CookieGeoLocationProvider>();

        if (configure is not null)
            builder.Services.Configure(configure);

        return builder;
    }
}
