namespace ClickView.GoodStuff.Queues.RabbitMq;

public class MessageWrapper<TData>
{
    public MessageWrapper(string id, TData data, long timestamp)
    {
        Id = id;
        Data = data;
        Timestamp = timestamp;
    }

    public string Id { get; init; }
    public TData Data { get; init; }
    public long Timestamp { get; init; }

    public static MessageWrapper<TData> New(TData data) => new
    (
        id: Guid.NewGuid().ToString(),
        data: data,
        timestamp: DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    );
}
