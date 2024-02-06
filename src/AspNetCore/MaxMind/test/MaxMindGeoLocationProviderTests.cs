namespace ClickView.GoodStuff.AspNetCore.MaxMind.Tests;

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Location;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

public class MaxMindGeoLocationProviderTests
{
    [Fact]
    public async Task GetGeoLocationInfoAsync_PrivateIpAddress_DoesNotCallMaxMind()
    {
        var client = Substitute.For<IMaxMindClient>();

        var httpContext = new DefaultHttpContext
        {
            Connection =
            {
                RemoteIpAddress = IPAddress.Parse("192.168.0.1")
            }
        };

        var provider = new MaxMindGeoLocationProvider(client, NullLogger<MaxMindGeoLocationProvider>.Instance);

        var locationInfo = await provider.GetGeoLocationInfoAsync(httpContext, CancellationToken.None);

        Assert.Null(locationInfo);
        await client.DidNotReceiveWithAnyArgs().CountryAsync(Arg.Any<IPAddress>());
    }
}
