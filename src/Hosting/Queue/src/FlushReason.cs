namespace ClickView.GoodStuff.Hosting.Queue;

internal enum FlushReason
{
    MaxIntervalReached,
    IntervalReached,
    BufferFull
}
