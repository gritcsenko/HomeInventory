namespace HomeInventory.Domain.Primitives;

public abstract class Entity<TSelf, TIdentity> : Equatable<TSelf>, IEntity<TSelf, TIdentity>
    where TIdentity : notnull, IIdentifierObject<TIdentity>
    where TSelf : notnull, Entity<TSelf, TIdentity>
{
    protected Entity(TIdentity id)
        : base(id) =>
        Id = id;

    public TIdentity Id { get; }
}
