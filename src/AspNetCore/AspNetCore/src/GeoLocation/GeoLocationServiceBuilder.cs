namespace ClickView.GoodStuff.AspNetCore.GeoLocation;

using Microsoft.Extensions.DependencyInjection;

public class GeoLocationServiceBuilder(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services;
}
