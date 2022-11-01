namespace HomeInventory.Domain.Primitives;

public abstract class AggregateRoot<TAggregate, TIdentity> : Entity<TAggregate, TIdentity>
    where TIdentity : IIdentifierObject<TIdentity>
    where TAggregate : AggregateRoot<TAggregate, TIdentity>
{
    protected AggregateRoot(TIdentity id)
        : base(id)
    {
    }
}
