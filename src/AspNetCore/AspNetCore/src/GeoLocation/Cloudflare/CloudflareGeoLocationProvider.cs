namespace ClickView.GoodStuff.AspNetCore.GeoLocation.Cloudflare;

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RequestHeader;

public class CloudflareGeoLocationProvider : RequestHeaderGeoLocationProvider
{
    private readonly ILogger<RequestHeaderGeoLocationProvider> _logger;

    public CloudflareGeoLocationProvider(ILogger<CloudflareGeoLocationProvider> logger,
        IOptions<CloudflareGeoLocationProviderOptions> options)
        : base(logger, options)
    {
        _logger = logger;
    }

    protected override string? GetCountryCode(HttpContext httpContext)
    {
        var countryCode = base.GetCountryCode(httpContext);

        // From their docs:
        // Cloudflare uses the XX country code when the country information is unknown.
        // https://developers.cloudflare.com/fundamentals/get-started/reference/http-request-headers/#cf-ipcountry
        if ("XX".Equals(countryCode, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Cloudflare could not determine the country of the request");
            return null;
        }

        return countryCode;
    }
}
