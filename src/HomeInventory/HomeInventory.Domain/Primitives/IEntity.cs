namespace HomeInventory.Domain.Primitives;

public interface IEntity
{
}

public interface IEntity<TEntity> : IEntity
    where TEntity : IEntity<TEntity>
{

}

public interface IEntity<TEntity, out TIdentifier> : IEntity<TEntity>, IEquatable<TEntity>
    where TIdentifier : IIdentifierObject<TIdentifier>
    where TEntity : IEntity<TEntity, TIdentifier>
{
    TIdentifier Id { get; }
}
