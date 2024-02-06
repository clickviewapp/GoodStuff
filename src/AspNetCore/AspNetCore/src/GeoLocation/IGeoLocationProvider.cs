namespace ClickView.GoodStuff.AspNetCore.GeoLocation;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public interface IGeoLocationProvider
{
    Task<GeoLocationInfo?> GetGeoLocationInfoAsync(HttpContext httpContext, CancellationToken cancellationToken = default);
}
