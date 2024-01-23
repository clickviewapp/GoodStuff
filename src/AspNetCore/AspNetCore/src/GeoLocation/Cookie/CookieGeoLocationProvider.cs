namespace ClickView.GoodStuff.AspNetCore.GeoLocation.Cookie;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

public class CookieGeoLocationProvider : IGeoLocationProvider
{
    private readonly CookieGeoLocationProviderOptions _options;

    public CookieGeoLocationProvider(IOptions<CookieGeoLocationProviderOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value;
    }

    public Task<GeoLocationInfo?> GetGeoLocationInfoAsync(HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        if (string.IsNullOrEmpty(_options.CookieName))
            return GeoLocationHelpers.NullValue;

        if (!httpContext.Request.Cookies.TryGetValue(_options.CookieName, out var value) || string.IsNullOrEmpty(value))
            return GeoLocationHelpers.NullValue;

        var values = value.Split(',');

        var countryCode = GetValue(values, 0);
        var continentCode = GetValue(values, 1);
        var subdivisionCode = GetValue(values, 2);

        // If we have no data, then return null so the next handler will be used
        if (string.IsNullOrEmpty(countryCode) &&
            string.IsNullOrEmpty(continentCode) &&
            string.IsNullOrEmpty(subdivisionCode))
            return GeoLocationHelpers.NullValue;

        return Task.FromResult<GeoLocationInfo?>(new GeoLocationInfo
        {
            CountryCode = countryCode,
            ContinentCode = continentCode,
            SubdivisionCode = subdivisionCode
        });

        static string? GetValue(IReadOnlyList<string> arr, int index)
        {
            if (arr.Count <= index)
                return null;

            var value = arr[index];

            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.ToUpper();
        }
    }
}
