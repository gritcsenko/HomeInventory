using HomeInventory.Domain.Events;

namespace HomeInventory.Domain.Primitives;
public abstract class AggregateRoot<TAggregate, TIdentity> : Entity<TAggregate, TIdentity>
    where TIdentity : notnull, IIdentifierObject<TIdentity>
    where TAggregate : notnull, AggregateRoot<TAggregate, TIdentity>
{
    private readonly SortedSet<IEvent> _events = new(EventComparer.Default);

    protected AggregateRoot(TIdentity id)
        : base(id)
    {
    }

    public IReadOnlyCollection<IEvent> Events => _events;

    protected bool Publish(IEvent @event) => _events.Add(@event);
}
