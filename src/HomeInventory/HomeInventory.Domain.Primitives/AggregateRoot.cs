namespace HomeInventory.Domain.Primitives;

public abstract class AggregateRoot<TSelf, TIdentity> : Entity<TSelf, TIdentity>, IAggregateRoot
    where TIdentity : IIdentifierObject<TIdentity>
    where TSelf : AggregateRoot<TSelf, TIdentity>
{
    private readonly EventsCollection _events = new();

    protected AggregateRoot(TIdentity id)
        : base(id)
    {
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.DomainEvents;

    protected void Raise(IDomainEvent @event) => _events.Push(@event);
}
