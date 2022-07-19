using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Aggregates;
public abstract class AggregateRoot<TAggregate, TIdentity> : Entity<TAggregate, TIdentity>
    where TIdentity : notnull, IIdentifierObject<TIdentity>
    where TAggregate : notnull, AggregateRoot<TAggregate, TIdentity>
{
    protected AggregateRoot(TIdentity id)
        : base(id)
    {
    }
}
