namespace ClickView.GoodStuff.AspNetCore.GeoLocation;

using System.Threading.Tasks;

internal static class GeoLocationHelpers
{
    internal static readonly Task<GeoLocationInfo?> NullValue = Task.FromResult<GeoLocationInfo?>(null);
}
