namespace HomeInventory.Domain.Primitives;

public abstract class AggregateRoot<TSelf, TIdentity> : Entity<TSelf, TIdentity>, IAggregateRoot
    where TIdentity : IIdentifierObject<TIdentity>
    where TSelf : AggregateRoot<TSelf, TIdentity>
{
    private readonly List<IDomainEvent> _events = new();

    protected AggregateRoot(TIdentity id)
        : base(id)
    {
    }

    public IReadOnlyCollection<IDomainEvent> GetAndClearEvents()
    {
        var events = _events.ToArray();
        _events.Clear();
        return events;
    }

    protected void AddEvent(IDomainEvent @event) => _events.Add(@event);
}
