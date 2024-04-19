namespace ClickView.GoodStuff.AspNetCore.GeoLocation.Cloudflare;

using RequestHeader;

public class CloudflareGeoLocationProviderOptions : RequestHeaderGeoLocationProviderOptions
{
    public CloudflareGeoLocationProviderOptions()
    {
        // https://developers.cloudflare.com/fundamentals/reference/http-request-headers/#cf-ipcountry
        CountryCodeHeader = "CF-IPCountry";

        // https://developers.cloudflare.com/rules/transform/managed-transforms/reference/#add-visitor-location-headers
        SubdivisionCodeHeader = "cf-region-code";
        ContinentCodeHeader = "cf-ipcontinent";
    }
}
