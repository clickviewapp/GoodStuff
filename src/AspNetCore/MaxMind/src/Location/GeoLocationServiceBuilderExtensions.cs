namespace ClickView.GoodStuff.AspNetCore.MaxMind.Location;

using System;
using GeoLocation;
using global::MaxMind.GeoIP2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

public static class GeoLocationServiceBuilderExtensions
{
    public static GeoLocationServiceBuilder AddMaxMind(this GeoLocationServiceBuilder builder, Action<MaxMindGeoLocationProviderOptions> configure)
    {
        builder.Services.Configure(configure);
        builder.Services.AddSingleton<IGeoLocationProvider, MaxMindGeoLocationProvider>();

        // MaxMind
        builder.Services.TryAddTransient<IMaxMindClient, MaxMindClient>();
        builder.Services.AddHttpClient<WebServiceClient>();
        builder.Services.AddOptions<WebServiceClientOptions>();
        builder.Services.AddSingleton<IPostConfigureOptions<WebServiceClientOptions>, MaxMindPostConfigure>();

        return builder;
    }
}
