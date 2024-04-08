namespace ClickView.GoodStuff.Queues.RabbitMq;

public class MessageWrapper<TData>
{
    public required string Id { get; init; }
    public required TData Data { get; init; }
    public required long Timestamp { get; init; }

    public static MessageWrapper<TData> Create(TData data) => new()
    {
        Id = Guid.NewGuid().ToString(),
        Data = data,
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    };
}
