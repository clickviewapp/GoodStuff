namespace ClickView.GoodStuff.AspNetCore.GeoLocation.Cookie;

using System;
using Microsoft.Extensions.DependencyInjection;
using RequestHeader;

public static class GeoLocationServiceBuilderExtensions
{
    public static GeoLocationServiceBuilder AddCookie(this GeoLocationServiceBuilder builder,
        Action<RequestHeaderGeoLocationProviderOptions>? configure = null)
    {
        builder.Services.AddSingleton<IGeoLocationProvider, RequestHeaderGeoLocationProvider>();

        if (configure is not null)
            builder.Services.Configure(configure);

        return builder;
    }
}
