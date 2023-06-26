using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class UserId : UlidIdentifierObject<UserId>
{
    internal UserId(Ulid value)
        : base(value)
    {
    }
}
