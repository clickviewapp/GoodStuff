namespace ClickView.GoodStuff.Queues.RabbitMq.Internal;

internal class ActiveSubscriptions
{
    private readonly List<SubscriptionContext> _list = new();
    private readonly object _lock = new();

    public void Add(SubscriptionContext context)
    {
        lock (_lock)
            _list.Add(context);
    }

    public void Remove(SubscriptionContext context)
    {
        lock (_lock)
            _list.Remove(context);
    }

    public IReadOnlyCollection<SubscriptionContext> GetAll()
    {
        lock (_lock)
            return _list.ToList();
    }
}
