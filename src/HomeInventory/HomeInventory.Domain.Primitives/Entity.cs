namespace HomeInventory.Domain.Primitives;

public abstract class Entity<TEntity, TIdentity> : Equatable<TEntity>, IEntity<TEntity, TIdentity>
    where TIdentity : IIdentifierObject<TIdentity>
    where TEntity : Entity<TEntity, TIdentity>
{
    protected Entity(TIdentity id)
        : base(id)
    {
        Id = id;
    }

    public TIdentity Id { get; }
}
