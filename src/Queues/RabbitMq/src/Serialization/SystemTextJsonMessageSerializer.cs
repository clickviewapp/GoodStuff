namespace ClickView.GoodStuff.Queues.RabbitMq.Serialization;

using System.Text.Json;

public class SystemTextJsonMessageSerializer : IMessageSerializer
{
    private readonly JsonSerializerOptions _options;

    public SystemTextJsonMessageSerializer() : this(GetDefaultOptions())
    {
    }

    public SystemTextJsonMessageSerializer(JsonSerializerOptions options)
    {
        _options = options;
    }

    public static SystemTextJsonMessageSerializer Default = new();

    private static JsonSerializerOptions GetDefaultOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public ReadOnlyMemory<byte> Serialize<TData>(MessageWrapper<TData> message)
    {
        return JsonSerializer.SerializeToUtf8Bytes(message, _options);
    }

    public MessageWrapper<TData>? Deserialize<TData>(ReadOnlySpan<byte> bytes)
    {
        return JsonSerializer.Deserialize<MessageWrapper<TData>>(bytes, _options);
    }
}
