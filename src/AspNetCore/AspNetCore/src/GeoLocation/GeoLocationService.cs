namespace ClickView.GoodStuff.AspNetCore.GeoLocation;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class GeoLocationService : IGeoLocationService
{
    private readonly IEnumerable<IGeoLocationProvider> _providers;
    private readonly ILogger<GeoLocationService> _logger;

    public GeoLocationService(IEnumerable<IGeoLocationProvider> providers, ILogger<GeoLocationService> logger)
    {
        _providers = providers;
        _logger = logger;
    }

    public async Task<GeoLocationInfo?> GetGeoLocationInfoAsync(HttpContext httpContext, CancellationToken cancellationToken = default)
    {
        foreach (var provider in _providers)
        {
            var geoInfo = await provider.GetGeoLocationInfoAsync(httpContext, cancellationToken);
            if (geoInfo == null)
                continue;

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Found GeoLocationInfo {@GeoInfo} using provider {Provider}",
                    geoInfo, provider.GetType().Name);
            }

            return geoInfo;
        }

        return null;
    }
}
