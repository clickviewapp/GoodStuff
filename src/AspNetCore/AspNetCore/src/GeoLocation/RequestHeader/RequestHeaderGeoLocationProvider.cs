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

        var countryCode = GetCountryCode(httpContext);
        var continentCode = GetContinentCode(httpContext);
        var subdivisionCode = GetSubdivisionCode(httpContext);

        // If we have no data, then return null so the next handler will be used
        if (string.IsNullOrEmpty(countryCode) &&
            string.IsNullOrEmpty(continentCode) &&
            string.IsNullOrEmpty(subdivisionCode))
            return Task.FromResult<GeoLocationInfo?>(null);

        return Task.FromResult<GeoLocationInfo?>(new GeoLocationInfo
        {
            CountryCode = countryCode?.ToUpper(),
            ContinentCode = continentCode?.ToUpper(),
            SubdivisionCode = subdivisionCode?.ToUpper()
        });
    }

    protected virtual string? GetCountryCode(HttpContext httpContext)
    {
        if (_options.CountryCodeHeader is null)
            return null;

        var countryCode = GetHeaderValue(httpContext, _options.CountryCodeHeader);

        if (string.IsNullOrEmpty(countryCode))
            _logger.LogDebug("No CountryCode found for header {HeaderKey}", _options.CountryCodeHeader);

        return countryCode;
    }

    protected virtual string? GetContinentCode(HttpContext httpContext)
    {
        if (_options.ContinentCodeHeader is null)
            return null;

        var continentCode = GetHeaderValue(httpContext, _options.ContinentCodeHeader);

        if (string.IsNullOrEmpty(continentCode))
            _logger.LogDebug("No ContinentCode found for header {HeaderKey}", _options.ContinentCodeHeader);

        return continentCode;
    }

    protected virtual string? GetSubdivisionCode(HttpContext httpContext)
    {
        if (_options.SubdivisionCodeHeader is null)
            return null;

        var subdivisionCode = GetHeaderValue(httpContext, _options.SubdivisionCodeHeader);

        if (string.IsNullOrEmpty(subdivisionCode))
            _logger.LogDebug("No SubdivisionCode found for header {HeaderKey}", _options.SubdivisionCodeHeader);

        return subdivisionCode;
    }

    protected static string? GetHeaderValue(HttpContext httpContext, string headerKey)
    {
        if (httpContext.Request.Headers.TryGetValue(headerKey, out var values) && values.Count > 0)
            return values[0];

        return null;
    }
}
