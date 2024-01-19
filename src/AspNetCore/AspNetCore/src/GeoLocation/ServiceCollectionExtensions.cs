namespace ClickView.GoodStuff.AspNetCore.GeoLocation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static GeoLocationServiceBuilder AddLocationServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IGeoLocationService, GeoLocationService>();

        return new GeoLocationServiceBuilder(services);
    }
}
