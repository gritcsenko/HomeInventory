namespace HomeInventory.Domain.Primitives;

public interface IEntity
{
}

public interface IEntity<TEntity> : IEntity
    where TEntity : notnull, IEntity<TEntity>
{

}

public interface IEntity<TEntity, TIdentifier> : IEntity<TEntity>, IEquatable<TEntity>
    where TIdentifier : notnull, IIdentifierObject<TIdentifier>
    where TEntity : notnull, IEntity<TEntity, TIdentifier>
{
    TIdentifier Id { get; }
}
