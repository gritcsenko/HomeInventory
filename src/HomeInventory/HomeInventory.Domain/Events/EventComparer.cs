namespace HomeInventory.Domain.Events;

public static class EventComparer
{
    public static IComparer<IEvent> Default { get; } = Comparer<IEvent>.Create(Compare);

    private static int Compare(IEvent? x, IEvent? y)
    {
        if (x is null)
        {
            return y is null ? 0 : -1;
        }
        if (y is null)
        {
            return 1;
        }

        return x.TimeStamp.CompareTo(y.TimeStamp);
    }
}
