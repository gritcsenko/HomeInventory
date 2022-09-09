using System.Collections.Concurrent;

namespace HomeInventory.Domain.Primitives;
public abstract class AggregateRoot<TAggregate, TIdentity> : Entity<TAggregate, TIdentity>
    where TIdentity : notnull, IIdentifierObject<TIdentity>
    where TAggregate : notnull, AggregateRoot<TAggregate, TIdentity>
{
    private readonly ConcurrentQueue<IDomainEvent> _events = new();

    protected AggregateRoot(TIdentity id)
        : base(id)
    {
    }

    public IReadOnlyCollection<IDomainEvent> Events => _events;

    protected void Publish(IDomainEvent @event) => _events.Enqueue(@event);
}
