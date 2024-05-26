using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.Primitives;

public abstract class AggregateRoot<TSelf, TIdentity>(TIdentity id) : Entity<TSelf, TIdentity>(id), IHasDomainEvents
    where TIdentity : IIdentifierObject<TIdentity>
    where TSelf : AggregateRoot<TSelf, TIdentity>
{
    private readonly EventsCollection _events = new();

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _events.DomainEvents;

    public void ClearDomainEvents() => _events.Clear();

    protected void AddDomainEvent(IDomainEvent @event) => _events.Push(@event);
}
