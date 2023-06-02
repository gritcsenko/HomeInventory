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

    public IReadOnlyCollection<IDomainEvent> PopAllEvents() => _events.PopAllDomainEvents();

    protected void Push(IDomainEvent @event) => _events.Push(@event);
}
