namespace ClickView.GoodStuff.Queues.RabbitMq;

public class MessageWrapper<TData>(string id, TData data, long timestamp)
{
    public string Id { get; init; } = id;
    public TData Data { get; init; } = data;
    public long Timestamp { get; init; } = timestamp;

    internal static MessageWrapper<TData> New(TData data) => new
    (
        id: Guid.NewGuid().ToString(),
        data: data,
        timestamp: DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    );
}
