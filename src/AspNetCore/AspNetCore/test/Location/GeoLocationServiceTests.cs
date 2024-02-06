namespace ClickView.GoodStuff.AspNetCore.Tests.Location;

using System.Threading;
using System.Threading.Tasks;
using GeoLocation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

public class GeoLocationServiceTests
{
    [Fact]
    public async Task GetGeoLocationInfoAsync_ALlValues_ReturnsGeoLocationInfo()
    {
        var httpContext = new DefaultHttpContext();

        var provider = Substitute.For<IGeoLocationProvider>();
        provider.GetGeoLocationInfoAsync(httpContext).Returns(new GeoLocationInfo
        {
            CountryCode = "TEST-1",
            SubdivisionCode = "NICE TEST",
            ContinentCode = "test-3"
        });

        var service = new GeoLocationService(new[] { provider }, NullLogger<GeoLocationService>.Instance);

        var location = await service.GetGeoLocationInfoAsync(httpContext, CancellationToken.None);

        Assert.NotNull(location);
        Assert.Equal("TEST-1", location.CountryCode);
        Assert.Equal("NICE TEST", location.SubdivisionCode);
        Assert.Equal("test-3", location.ContinentCode);
    }
}
