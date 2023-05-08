namespace HomeInventory.Domain.Primitives;

public abstract class Entity<TEntity, TIdentity> : Equatable<TEntity>, IEntity<TEntity, TIdentity>
    where TIdentity : notnull, IIdentifierObject<TIdentity>
    where TEntity : notnull, Entity<TEntity, TIdentity>
{
    protected Entity(TIdentity id)
        : base(id) =>
        Id = id;

    public TIdentity Id { get; }
}
