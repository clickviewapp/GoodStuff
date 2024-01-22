namespace ClickView.GoodStuff.AspNetCore.Tests.Location;

using System.Threading;
using System.Threading.Tasks;
using GeoLocation.Cloudflare;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

public class CloudflareGeoLocationProviderTests
{
   [Fact]
    public async Task GetGeoLocationInfoAsync_XXIPCountry_ReturnsNull()
    {
        var options = new CloudflareGeoLocationProviderOptions();
        var provider = new CloudflareGeoLocationProvider(
            NullLogger<CloudflareGeoLocationProvider>.Instance,
            new OptionsWrapper<CloudflareGeoLocationProviderOptions>(options));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["CF-IPCountry"] = "XX";

        var locationInfo = await provider.GetGeoLocationInfoAsync(httpContext, CancellationToken.None);

        Assert.Null(locationInfo);
    }

    [Fact]
    public async Task GetGeoLocationInfoAsync_IPCountrySet_ReturnsUpperCase()
    {
        var options = new CloudflareGeoLocationProviderOptions();
        var provider = new CloudflareGeoLocationProvider(
            NullLogger<CloudflareGeoLocationProvider>.Instance,
            new OptionsWrapper<CloudflareGeoLocationProviderOptions>(options));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["CF-IPCountry"] = "test";

        var locationInfo = await provider.GetGeoLocationInfoAsync(httpContext, CancellationToken.None);

        Assert.NotNull(locationInfo);
        Assert.Equal("TEST", locationInfo.CountryCode);
    }
}
