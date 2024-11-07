namespace ClickView.GoodStuff.Queues.RabbitMq.Serialization;

using System.Text;
using Newtonsoft.Json;

public class NewtonsoftJsonMessageSerializer : IMessageSerializer
{
    private readonly JsonSerializerSettings _settings;

    public NewtonsoftJsonMessageSerializer() :this(GetDefaultSettings())
    {
    }

    public NewtonsoftJsonMessageSerializer(JsonSerializerSettings settings)
    {
        _settings = settings;
    }

    private static JsonSerializerSettings GetDefaultSettings()
    {
        return new JsonSerializerSettings();
    }

    public ReadOnlyMemory<byte> Serialize<TData>(MessageWrapper<TData> message)
    {
        var json = JsonConvert.SerializeObject(message, _settings);
        return Encoding.UTF8.GetBytes(json);
    }

    public MessageWrapper<TData>? Deserialize<TData>(ReadOnlySpan<byte> bytes)
    {
        var json = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject<MessageWrapper<TData>>(json, _settings);
    }
}
