namespace ClickView.GoodStuff.AspNetCore.MaxMind.Location;

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using global::MaxMind.GeoIP2.Responses;

internal interface IMaxMindClient
{
    Task<CountryResponse> CountryAsync(IPAddress ipAddress, CancellationToken cancellationToken = default);
}