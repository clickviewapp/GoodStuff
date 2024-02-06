namespace ClickView.GoodStuff.AspNetCore.MaxMind.Location;

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using global::MaxMind.GeoIP2;
using global::MaxMind.GeoIP2.Responses;

internal class MaxMindClient : IMaxMindClient
{
    private readonly WebServiceClient _client;

    public MaxMindClient(WebServiceClient client)
    {
        _client = client;
    }

    public Task<CountryResponse> CountryAsync(IPAddress ipAddress, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _client.CountryAsync(ipAddress);
    }
}