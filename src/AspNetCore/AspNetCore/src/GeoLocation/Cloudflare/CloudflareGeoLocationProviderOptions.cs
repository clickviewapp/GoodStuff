namespace ClickView.GoodStuff.AspNetCore.GeoLocation.Cloudflare;

using RequestHeader;

public class CloudflareGeoLocationProviderOptions : RequestHeaderGeoLocationProviderOptions
{
    public CloudflareGeoLocationProviderOptions()
    {
        CountryCodeHeader = "CF-IPCountry";
    }
}
