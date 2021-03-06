using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Entities;
public interface IEntity<TEntity>
    where TEntity : notnull, IEntity<TEntity>
{

}

public interface IEntity<TEntity, TIdentifier> : IEntity<TEntity>, IEquatable<TEntity>
    where TIdentifier : notnull, IIdentifierObject<TIdentifier>
    where TEntity : notnull, IEntity<TEntity, TIdentifier>
{
    TIdentifier Id { get; }
}
