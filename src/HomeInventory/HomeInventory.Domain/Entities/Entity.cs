using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Entities;

public class Entity<TEntity, TIdentity> : IEntity<TEntity, TIdentity>
    where TIdentity : notnull, IIdentityValue
    where TEntity : notnull, Entity<TEntity, TIdentity>
{
    public Entity(TIdentity id) => Id = id;

    public TIdentity Id { get; }

    public bool Equals(TEntity? other) => ReferenceEquals(other, this) || (other is not null && EqualsCore(other));

    public override bool Equals(object? obj) => obj is TEntity entity && Equals(entity);

    public override int GetHashCode() => Id.GetHashCode();

    protected virtual bool EqualsCore(TEntity other) => Id.Equals(other.Id);
}
