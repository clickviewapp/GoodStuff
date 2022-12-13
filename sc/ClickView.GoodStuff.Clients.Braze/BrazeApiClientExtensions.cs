namespace ClickView.GoodStuff.Clients.Braze;

using Models;
using Responses;

public static class BrazeApiClientExtensions
{
    /// <summary>
    /// Record custom events, purchases, and update user profile attributes
    /// </summary>
    /// <param name="brazeApiClient"></param>
    /// <param name="attribute"></param>
    /// <param name="brazeEvent"></param>
    /// <param name="purchase"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static Task<UserTrackResponse> UserTrackAsync(
        this IBrazeApiClient brazeApiClient,
        BrazeUserAttribute? attribute = default, 
        BrazeEvent? brazeEvent = default,
        BrazePurchase? purchase = default,
        CancellationToken token = default)
    {
        var attributes = attribute == null ? Enumerable.Empty<BrazeUserAttribute>() : new[] { attribute };
        var events = brazeEvent == null ? Enumerable.Empty<BrazeEvent>() : new[] { brazeEvent };
        var purchases = purchase == null ? Enumerable.Empty<BrazePurchase>() : new[] { purchase };

        return brazeApiClient.UserTrackAsync(attributes, events, purchases, token);
    }
}