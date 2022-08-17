namespace HomeInventory.Domain.Events;

public class EventComparer : Comparer<IEvent>
{
    public static new IComparer<IEvent> Default { get; } = new EventComparer();

    public override int Compare(IEvent? x, IEvent? y)
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
