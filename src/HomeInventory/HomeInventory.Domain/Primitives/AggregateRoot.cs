namespace HomeInventory.Domain.Primitives;
public abstract class AggregateRoot<TAggregate, TIdentity> : Entity<TAggregate, TIdentity>
    where TIdentity : notnull, IIdentifierObject<TIdentity>
    where TAggregate : notnull, AggregateRoot<TAggregate, TIdentity>
{
    protected AggregateRoot(TIdentity id)
        : base(id)
    {
    }
}
