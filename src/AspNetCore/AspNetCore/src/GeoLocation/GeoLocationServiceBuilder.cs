namespace ClickView.GoodStuff.AspNetCore.GeoLocation;

using Microsoft.Extensions.DependencyInjection;

public class GeoLocationServiceBuilder
{
    public GeoLocationServiceBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}
