namespace ClickView.GoodStuff.AspNetCore.GeoLocation;

public class GeoLocationInfo
{
    /// <summary>
    /// 2 letter ISO 3166-1 country code
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// The continent code (eg NA)
    /// </summary>
    public string? ContinentCode { get; set; }

    /// <summary>
    /// The subdivision code for the country code (eg NSW)
    /// </summary>
    public string? SubdivisionCode { get; set; }
}
