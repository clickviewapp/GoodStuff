namespace ClickView.GoodStuff.Clients.Braze.Requests;

using Extensions.RestClient.Requests;
using Extensions.RestClient.Serialization;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Responses;

internal class UserTrackRequest : RestClientRequest<UserTrackResponse>
{
    public UserTrackRequest(
        IEnumerable<BrazeUserAttribute> attributes, 
        IEnumerable<BrazeEvent> events, 
        IEnumerable<BrazePurchase> purchases) : base(HttpMethod.Post, "users/track")
    {
        Serializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        });

        AddBody(new { attributes, events, purchases });
    }
}