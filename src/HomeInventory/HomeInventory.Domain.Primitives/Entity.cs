namespace HomeInventory.Domain.Primitives;

public abstract class Entity<TSelf, TIdentity>(TIdentity id) : Equatable<TSelf>(id), IEntity<TSelf, TIdentity>
    where TIdentity : notnull, IIdentifierObject<TIdentity>
    where TSelf : notnull, Entity<TSelf, TIdentity>
{
    public TIdentity Id { get; } = id;
}
