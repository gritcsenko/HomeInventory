namespace HomeInventory.Domain.Primitives;

public abstract class Entity<TEntity, TIdentity> : IEntity<TEntity, TIdentity>
    where TIdentity : notnull, IIdentifierObject<TIdentity>
    where TEntity : notnull, Entity<TEntity, TIdentity>
{
    protected Entity(TIdentity id) => Id = id;

    public TIdentity Id { get; }

    public static bool operator ==(Entity<TEntity, TIdentity> a, TEntity b) => a.Equals(b);

    public static bool operator !=(Entity<TEntity, TIdentity> a, TEntity b) => !a.Equals(b);

    public bool Equals(TEntity? other) => ReferenceEquals(other, this) || other is not null && Id.Equals(other.Id);

    public override bool Equals(object? obj) => Equals(obj as TEntity);

    public override int GetHashCode() => Id.GetHashCode();
}
