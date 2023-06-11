namespace HomeInventory.Domain.Primitives;

public abstract class AggregateRoot<TSelf, TIdentity> : Entity<TSelf, TIdentity>, IHasDomainEvents
    where TIdentity : IIdentifierObject<TIdentity>
    where TSelf : AggregateRoot<TSelf, TIdentity>
{
    private readonly EventsCollection _events = new();

    protected AggregateRoot(TIdentity id)
        : base(id)
    {
    }

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _events.DomainEvents;

    public void ClearDomainEvents() => _events.Clear();

    protected void AddDomainEvent(IDomainEvent @event) => _events.Push(@event);
}
