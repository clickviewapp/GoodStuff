namespace ClickView.GoodStuff.Clients.Braze;

using Models;
using Responses;

public interface IBrazeApiClient
{
    /// <summary>
    /// Record custom events, purchases, and update user profile attributes
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="events"></param>
    /// <param name="purchases"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<UserTrackResponse> UserTrackAsync(IEnumerable<BrazeUserAttribute> attributes, IEnumerable<BrazeEvent> events,
        IEnumerable<BrazePurchase> purchases, CancellationToken token = default);
}