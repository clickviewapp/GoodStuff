namespace ClickView.GoodStuff.AspNetCore.GeoLocation.Cloudflare;

using System;
using Microsoft.Extensions.DependencyInjection;

public static class GeoLocationServiceBuilderExtensions
{
    public static GeoLocationServiceBuilder AddCloudflare(this GeoLocationServiceBuilder builder,
        Action<CloudflareGeoLocationProviderOptions>? configure = null)
    {
        builder.Services.AddSingleton<IGeoLocationProvider, CloudflareGeoLocationProvider>();

        if (configure is not null)
            builder.Services.Configure(configure);

        return builder;
    }
}
