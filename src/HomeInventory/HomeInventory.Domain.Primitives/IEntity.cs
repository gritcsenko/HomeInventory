using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.Primitives;

public interface IEntityWithId<out TIdentifier>
    where TIdentifier : IIdentifierObject<TIdentifier>
{
    TIdentifier Id { get; }
}

public interface IEntity<TSelf> : IEquatable<TSelf>
    where TSelf : IEntity<TSelf>
{
}

public interface IEntity<TSelf, out TIdentifier> : IEntity<TSelf>, IEntityWithId<TIdentifier>
    where TIdentifier : IIdentifierObject<TIdentifier>
    where TSelf : IEntity<TSelf, TIdentifier>
{
}
