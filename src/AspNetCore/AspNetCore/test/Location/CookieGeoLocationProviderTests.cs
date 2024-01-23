namespace ClickView.GoodStuff.AspNetCore.Tests.Location;

using System.Threading.Tasks;
using GeoLocation.Cookie;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Xunit;

public class CookieGeoLocationProviderTests
{
    [Fact]
    public async Task GetGeoLocationInfoAsync_AllValuesExist_ReturnsCorrectValues()
    {
        var options = new CookieGeoLocationProviderOptions
        {
            CookieName = "test-cookie"
        };
        var provider = new CookieGeoLocationProvider(new OptionsWrapper<CookieGeoLocationProviderOptions>(options));
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Cookie = "test-cookie=aa%2Cbb%2Ccc";

        var locationInfo = await provider.GetGeoLocationInfoAsync(httpContext);

        Assert.NotNull(locationInfo);
        Assert.Equal("AA", locationInfo.CountryCode);
        Assert.Equal("BB", locationInfo.ContinentCode);
        Assert.Equal("CC", locationInfo.SubdivisionCode);
    }

    [Fact]
    public async Task GetGeoLocationInfoAsync_OnlyCountryCode_ReturnsCountryCode()
    {
        var options = new CookieGeoLocationProviderOptions
        {
            CookieName = "test-cookie"
        };
        var provider = new CookieGeoLocationProvider(new OptionsWrapper<CookieGeoLocationProviderOptions>(options));
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Cookie = "test-cookie=aa";

        var locationInfo = await provider.GetGeoLocationInfoAsync(httpContext);

        Assert.NotNull(locationInfo);
        Assert.Equal("AA", locationInfo.CountryCode);
        Assert.Null(locationInfo.ContinentCode);
        Assert.Null(locationInfo.SubdivisionCode);
    }

    [Fact]
    public async Task GetGeoLocationInfoAsync_NoCookie_ReturnsNull()
    {
        var options = new CookieGeoLocationProviderOptions
        {
            CookieName = "test-cookie"
        };
        var provider = new CookieGeoLocationProvider(new OptionsWrapper<CookieGeoLocationProviderOptions>(options));
        var httpContext = new DefaultHttpContext();

        var locationInfo = await provider.GetGeoLocationInfoAsync(httpContext);

        Assert.Null(locationInfo);
    }

    [Fact]
    public async Task GetGeoLocationInfoAsync_EmptyCookie_ReturnsNull()
    {
        var options = new CookieGeoLocationProviderOptions
        {
            CookieName = "test-cookie"
        };
        var provider = new CookieGeoLocationProvider(new OptionsWrapper<CookieGeoLocationProviderOptions>(options));
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Cookie = "test-cookie=";

        var locationInfo = await provider.GetGeoLocationInfoAsync(httpContext);

        Assert.Null(locationInfo);
    }

    [Fact]
    public async Task GetGeoLocationInfoAsync_EmptyValues_ReturnsNull()
    {
        var options = new CookieGeoLocationProviderOptions
        {
            CookieName = "test-cookie"
        };
        var provider = new CookieGeoLocationProvider(new OptionsWrapper<CookieGeoLocationProviderOptions>(options));
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Cookie = "test-cookie=%2C";

        var locationInfo = await provider.GetGeoLocationInfoAsync(httpContext);

        Assert.Null(locationInfo);
    }

    [Fact]
    public async Task GetGeoLocationInfoAsync_OnlySubdivisionCode_ReturnsSubdivisionCode()
    {
        var options = new CookieGeoLocationProviderOptions
        {
            CookieName = "test-cookie"
        };
        var provider = new CookieGeoLocationProvider(new OptionsWrapper<CookieGeoLocationProviderOptions>(options));
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Cookie = "test-cookie=%2C%2Ctest";

        var locationInfo = await provider.GetGeoLocationInfoAsync(httpContext);

        Assert.NotNull(locationInfo);
        Assert.Null(locationInfo.CountryCode);
        Assert.Null(locationInfo.ContinentCode);
        Assert.Equal("TEST", locationInfo.SubdivisionCode);
    }
}
