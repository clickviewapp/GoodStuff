namespace ClickView.GoodStuff.AspNetCore.MaxMind.Location;

public class MaxMindGeoLocationProviderOptions
{
    public int AccountId { get; set; }
    public string LicenseKey { get; set; } = string.Empty;
}
