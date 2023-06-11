namespace HomeInventory.Domain.Primitives;

internal sealed class EventsCollection
{
    private readonly List<IDomainEvent> _events = new();

    public EventsCollection()
    {
    }

    public void Push(IDomainEvent domainEvent) => _events.Add(domainEvent);

    internal void Clear() => _events.Clear();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _events;
}
