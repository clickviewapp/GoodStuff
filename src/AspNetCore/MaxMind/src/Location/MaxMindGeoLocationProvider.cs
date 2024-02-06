namespace ClickView.GoodStuff.AspNetCore.MaxMind.Location;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Extensions.Primitives.Extensions;
using Extensions.Utilities.Threading;
using GeoLocation;
using global::MaxMind.GeoIP2.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

internal class MaxMindGeoLocationProvider : IGeoLocationProvider
{
    private readonly TaskSingle<IPAddress> _taskSingle = new();

    private readonly ILogger<MaxMindGeoLocationProvider> _logger;
    private readonly IMaxMindClient _client;

    public MaxMindGeoLocationProvider(
        IMaxMindClient client,
        ILogger<MaxMindGeoLocationProvider> logger)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<GeoLocationInfo?> GetGeoLocationInfoAsync(HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        try
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress;

            // Cant get a country from local addresses
            if (ipAddress is null || IPAddress.IsLoopback(ipAddress))
                return null;

            // Ignore private addresses
            if (ipAddress.IsPrivate())
                return null;

            // Fetch from maxmind
            return await GetLocationAsync(ipAddress, cancellationToken);
        }
        catch (GeoIP2Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch country from MaxMind");
            return null;
        }
    }

    private Task<GeoLocationInfo?> GetLocationAsync(IPAddress ipAddress, CancellationToken cancellationToken)
    {
        return _taskSingle.RunAsync(ipAddress, async (_, ct) =>
        {
            var response = await _client.CountryAsync(ipAddress, ct);

            if (response.Country.IsoCode is null)
                return null;

            return new GeoLocationInfo
            {
                CountryCode = response.Country.IsoCode.ToUpper(),
                ContinentCode = response.Continent.Code?.ToUpper(),
                SubdivisionCode = null
            };
        }, cancellationToken);
    }
}
