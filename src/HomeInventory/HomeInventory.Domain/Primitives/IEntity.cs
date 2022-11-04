namespace HomeInventory.Domain.Primitives;

public interface IEntity
{
}

public interface IEntityWithId<out TIdentifier> : IEntity
    where TIdentifier : IIdentifierObject<TIdentifier>
{
    TIdentifier Id { get; }
}

public interface IEntity<TEntity> : IEntity
    where TEntity : IEntity<TEntity>
{

}

public interface IEntity<TEntity, out TIdentifier> : IEntity<TEntity>, IEntityWithId<TIdentifier>, IEquatable<TEntity>
    where TIdentifier : IIdentifierObject<TIdentifier>
    where TEntity : IEntity<TEntity, TIdentifier>
{
}
