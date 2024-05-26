using DotNext;
using HomeInventory.Domain.Primitives.Ids;
using Visus.Cuid;

namespace HomeInventory.Domain.ValueObjects;

public sealed class StorageAreaId : CuidIdentifierObject<StorageAreaId>, ICuidBuildable<StorageAreaId>
{
    private StorageAreaId(Cuid value)
        : base(value)
    {
    }

    public static Result<StorageAreaId> CreateFrom(Cuid value) => Result.FromValue(new StorageAreaId(value));
}
