namespace ClickView.GoodStuff.AspNetCore.GeoLocation.RequestHeader;

public class RequestHeaderGeoLocationProviderOptions
{
    /// <summary>
    /// The header key for the country code
    /// </summary>
    public string? CountryCodeHeader { get; set; }

    /// <summary>
    /// The header key for the region code
    /// </summary>
    public string? RegionCodeHeader { get; set; }

    /// <summary>
    /// The header key for the continent code
    /// </summary>
    public string? ContinentCodeHeader { get; set; }
}
