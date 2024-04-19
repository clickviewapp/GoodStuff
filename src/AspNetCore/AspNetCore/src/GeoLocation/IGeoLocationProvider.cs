namespace ClickView.GoodStuff.AspNetCore.GeoLocation;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public interface IGeoLocationProvider
{
    ValueTask<GeoLocationInfo?> GetGeoLocationInfoAsync(HttpContext httpContext, CancellationToken cancellationToken = default);
}
