namespace HomeInventory.Domain.Primitives;

public interface IEntity
{
}

public interface IEntityWithId<out TIdentifier> : IEntity
    where TIdentifier : IIdentifierObject<TIdentifier>
{
    TIdentifier Id { get; }
}

public interface IEntity<TSelf> : IEntity
    where TSelf : IEntity<TSelf>
{

}

public interface IEntity<TSelf, out TIdentifier> : IEntity<TSelf>, IEntityWithId<TIdentifier>, IEquatable<TSelf>
    where TIdentifier : IIdentifierObject<TIdentifier>
    where TSelf : IEntity<TSelf, TIdentifier>
{
}
