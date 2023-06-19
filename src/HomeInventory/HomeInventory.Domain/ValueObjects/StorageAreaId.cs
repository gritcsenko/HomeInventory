using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class StorageAreaId : UlidIdentifierObject<StorageAreaId>
{
    internal StorageAreaId(Ulid value)
        : base(value)
    {
    }
}
