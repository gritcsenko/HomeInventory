namespace HomeInventory.Domain.Primitives;

internal sealed class EventsCollection
{
    private readonly List<IDomainEvent> _events = new();

    public EventsCollection()
    {
    }

    public void Push(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    public IReadOnlyCollection<IDomainEvent> PopAllDomainEvents()
    {
        var events = _events.ToArray();
        _events.Clear();
        return events;
    }
}
