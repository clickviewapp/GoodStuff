namespace ClickView.GoodStuff.Hosting.Queue;

using System.Collections;
using Queues.RabbitMq;

internal class MessageContextScope<T>(MessageContext<T> context) : IReadOnlyList<KeyValuePair<string, object>>
{
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        for (var i = 0; i < Count; ++i)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => 1;

    public KeyValuePair<string, object> this[int index] => index switch
    {
        0 => new KeyValuePair<string, object>("MessageId", context.Id),
        _ => throw new IndexOutOfRangeException(nameof(index)),
    };
}
