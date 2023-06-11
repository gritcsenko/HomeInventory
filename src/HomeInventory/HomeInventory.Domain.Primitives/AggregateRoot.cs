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

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _events.DomainEvents;

    public void OnEventsSaved() => _events.Clear();

    protected void Publish(IDomainEvent @event) => _events.Push(@event);
}
