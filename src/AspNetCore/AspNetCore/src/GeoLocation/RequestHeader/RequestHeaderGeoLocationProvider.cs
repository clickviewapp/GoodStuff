namespace ClickView.GoodStuff.AspNetCore.GeoLocation.RequestHeader;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class RequestHeaderGeoLocationProvider : IGeoLocationProvider
{
    private readonly ILogger<RequestHeaderGeoLocationProvider> _logger;
    private readonly RequestHeaderGeoLocationProviderOptions _options;

    public RequestHeaderGeoLocationProvider(ILogger<RequestHeaderGeoLocationProvider> logger,
        IOptions<RequestHeaderGeoLocationProviderOptions> options)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(options);

        _logger = logger;
        _options = options.Value;
    }

    public Task<GeoLocationInfo?> GetGeoLocationInfoAsync(HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        string? countryCode = null;
        string? continentCode = null;
        string? regionCode = null;

        if (_options.CountryCodeHeader is not null)
        {
            countryCode = GetHeaderValue(httpContext, _options.CountryCodeHeader);

            if (string.IsNullOrEmpty(countryCode))
                _logger.LogDebug("No CountryCode found for header {HeaderKey}", _options.CountryCodeHeader);
        }

        if (_options.ContinentCodeHeader is not null)
        {
            continentCode = GetHeaderValue(httpContext, _options.ContinentCodeHeader);

            if (string.IsNullOrEmpty(continentCode))
                _logger.LogDebug("No ContinentCode found for header {HeaderKey}", _options.ContinentCodeHeader);
        }

        if (_options.RegionCodeHeader is not null)
        {
            regionCode = GetHeaderValue(httpContext, _options.RegionCodeHeader);

            if (string.IsNullOrEmpty(regionCode))
                _logger.LogDebug("No RegionCode found for header {HeaderKey}", _options.RegionCodeHeader);
        }

        // If we have no data, then return null so the next handler will be used
        if (string.IsNullOrEmpty(countryCode) &&
            string.IsNullOrEmpty(continentCode) &&
            string.IsNullOrEmpty(regionCode))
            return Task.FromResult<GeoLocationInfo?>(null);

        return Task.FromResult<GeoLocationInfo?>(new GeoLocationInfo
        {
            CountryCode = countryCode?.ToUpper(),
            ContinentCode = continentCode?.ToUpper(),
            SubdivisionCode = regionCode?.ToUpper()
        });
    }

    private static string? GetHeaderValue(HttpContext httpContext, string headerKey)
    {
        if (httpContext.Request.Headers.TryGetValue(headerKey, out var values) && values.Count > 0)
            return values[0];

        return null;
    }
}
