namespace ClickView.GoodStuff.Queues.RabbitMq.Internal;

/// <summary>
/// Allows waiting for a count to reach 0
/// </summary>
internal class CountWaiter
{
    private readonly object _pendingLock = new();
    private readonly List<TaskCompletionSource> _pendingAwaiters = new();
    private int _count;

    public Task WaitAsync()
    {
        if (_count == 0)
            return Task.CompletedTask;

        var tcs = new TaskCompletionSource();

        lock (_pendingLock)
            _pendingAwaiters.Add(tcs);

        return tcs.Task;
    }

    public void Increment()
    {
        Interlocked.Increment(ref _count);
    }

    public void Decrement()
    {
        if (Interlocked.Decrement(ref _count) != 0) return;

        lock (_pendingLock)
        {
            if (_pendingAwaiters.Count <= 0) return;

            foreach (var awaiter in _pendingAwaiters)
                awaiter.SetResult();

            _pendingAwaiters.Clear();
        }
    }
}
