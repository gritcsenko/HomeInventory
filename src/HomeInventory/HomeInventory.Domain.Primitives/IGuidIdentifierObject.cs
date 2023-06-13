using DotNext;

namespace HomeInventory.Domain.Primitives;

public interface IGuidIdentifierObject<TSelf> : IIdentifierObject<TSelf>, IBuildable<TSelf, GuidIdentifierObjectBuilder<TSelf>>
    where TSelf : class, IGuidIdentifierObject<TSelf>
{
    Guid Value { get; }
}
