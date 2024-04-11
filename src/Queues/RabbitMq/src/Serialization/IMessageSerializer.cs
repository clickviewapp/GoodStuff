namespace ClickView.GoodStuff.Queues.RabbitMq.Serialization;

public interface IMessageSerializer
{
    public ReadOnlyMemory<byte> Serialize<TData>(MessageWrapper<TData> message);
    public MessageWrapper<TData>? Deserialize<TData>(ReadOnlySpan<byte> bytes);
}
