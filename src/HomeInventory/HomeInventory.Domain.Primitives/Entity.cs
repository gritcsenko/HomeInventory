using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.Primitives;

public abstract class Entity<TSelf, TIdentity>(TIdentity id) : Equatable<TSelf>(id), IEntity<TSelf, TIdentity>
    where TIdentity : IIdentifierObject<TIdentity>
    where TSelf : Entity<TSelf, TIdentity>
{
    public TIdentity Id { get; } = id;
}
