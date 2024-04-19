namespace ClickView.GoodStuff.AspNetCore.GeoLocation.RequestHeader;

using System;
using Microsoft.Extensions.DependencyInjection;

public static class GeoLocationServiceBuilderExtensions
{
    public static GeoLocationServiceBuilder AddRequestHeader(this GeoLocationServiceBuilder builder,
        Action<RequestHeaderGeoLocationProviderOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddSingleton<IGeoLocationProvider, RequestHeaderGeoLocationProvider>();

        if (configure is not null)
            builder.Services.Configure(configure);

        return builder;
    }
}
