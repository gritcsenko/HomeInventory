using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;
public sealed class UserId : GuidIdentifierObject<UserId>
{
    internal UserId(Guid value)
        : base(value)
    {
    }
}
