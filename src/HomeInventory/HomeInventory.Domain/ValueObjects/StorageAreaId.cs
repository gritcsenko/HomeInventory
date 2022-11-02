using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class StorageAreaId : GuidIdentifierObject<StorageAreaId>
{
    public StorageAreaId(Guid value)
        : base(value)
    {
    }
}
