namespace ClickView.GoodStuff.Clients.Braze;

using Extensions.RestClient;
using Extensions.RestClient.Authentication;
using Models;
using Requests;
using Responses;

public class BrazeApiClient : IBrazeApiClient
{
    private readonly RestClient _client;

    public BrazeApiClient(Uri baseAddress, IAuthenticator authenticator)
    {
        _client = new RestClient(baseAddress, new RestClientOptions
        {
            Authenticator = authenticator
        });
    }

    /// <inheritdoc />
    public async Task<UserTrackResponse> UserTrackAsync(IEnumerable<BrazeUserAttribute> attributes, 
        IEnumerable<BrazeEvent> events, IEnumerable<BrazePurchase> purchases, CancellationToken token = default)
    {
        var request = new UserTrackRequest(attributes, events, purchases);
        var response = await _client.ExecuteAsync(request, token);

        return response.Data;
    }
}