using HomeInventory.Domain.Primitives.Events;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Domain.Primitives;

public abstract class AggregateRoot<TSelf, TIdentity>(TIdentity id) : Entity<TSelf, TIdentity>(id), IHasDomainEvents
    where TIdentity : IIdentifierObject<TIdentity>
    where TSelf : AggregateRoot<TSelf, TIdentity>
{
    private readonly EventsCollection _events = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _events;

    public void ClearDomainEvents() => _events.Clear();

    protected void AddDomainEvent(IDomainEvent @event) => _events.Add(@event);
}
