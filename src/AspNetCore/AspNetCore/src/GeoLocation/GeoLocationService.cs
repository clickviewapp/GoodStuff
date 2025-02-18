namespace ClickView.GoodStuff.AspNetCore.GeoLocation;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class GeoLocationService(IEnumerable<IGeoLocationProvider> providers, ILogger<GeoLocationService> logger)
    : IGeoLocationService
{
    public async Task<GeoLocationInfo?> GetGeoLocationInfoAsync(HttpContext httpContext, CancellationToken cancellationToken = default)
    {
        foreach (var provider in providers)
        {
            var geoInfo = await provider.GetGeoLocationInfoAsync(httpContext, cancellationToken);
            if (geoInfo == null)
                continue;

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Found GeoLocationInfo {@GeoInfo} using provider {Provider}",
                    geoInfo, provider.GetType().Name);
            }

            return geoInfo;
        }

        return null;
    }
}
