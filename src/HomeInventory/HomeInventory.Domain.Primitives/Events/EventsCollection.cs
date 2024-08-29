using HomeInventory.Domain.Primitives.Messages;
using System.Collections;

namespace HomeInventory.Domain.Primitives.Events;

internal sealed class EventsCollection : IReadOnlyCollection<IDomainEvent>
{
    private readonly List<IDomainEvent> _events = [];

    public int Count => _events.Count;

    public void Add(IDomainEvent domainEvent) => _events.Add(domainEvent);

    public void Clear() => _events.Clear();

    public IEnumerator<IDomainEvent> GetEnumerator() => _events.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _events.GetEnumerator();
}
