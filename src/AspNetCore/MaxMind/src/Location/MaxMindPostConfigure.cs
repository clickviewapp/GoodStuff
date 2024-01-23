namespace ClickView.GoodStuff.AspNetCore.MaxMind.Location;

using global::MaxMind.GeoIP2;
using Microsoft.Extensions.Options;

internal class MaxMindPostConfigure : IPostConfigureOptions<WebServiceClientOptions>
{
    private readonly MaxMindGeoLocationProviderOptions _options;

    public MaxMindPostConfigure(IOptions<MaxMindGeoLocationProviderOptions> options)
    {
        _options = options.Value;
    }

    public void PostConfigure(string? name, WebServiceClientOptions options)
    {
        options.AccountId = _options.AccountId;
        options.LicenseKey = _options.LicenseKey;
    }
}
