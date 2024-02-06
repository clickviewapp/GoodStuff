namespace ClickView.GoodStuff.AspNetCore.GeoLocation;

public record GeoLocationInfo
{
    /// <summary>
    /// 2 letter ISO 3166-1 country code
    /// </summary>
    public string? CountryCode { get; init; }

    /// <summary>
    /// The continent code (eg NA)
    /// </summary>
    public string? ContinentCode { get; init; }

    /// <summary>
    /// The subdivision code for the country code (eg NSW)
    /// </summary>
    public string? SubdivisionCode { get; init; }
}
