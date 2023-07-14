namespace HomeInventory.Domain.Primitives;

public interface IUlidIdentifierObject<TSelf> : IIdentifierObject<TSelf>, IOptionalBuildable<TSelf, UlidIdentifierObjectBuilder<TSelf>>
    where TSelf : class, IUlidIdentifierObject<TSelf>
{
    Ulid Value { get; }
}
