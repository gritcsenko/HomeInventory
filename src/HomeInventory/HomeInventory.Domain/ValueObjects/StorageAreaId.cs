using DotNext;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class StorageAreaId : UlidIdentifierObject<StorageAreaId>, IUlidBuildable<StorageAreaId>
{
    private StorageAreaId(Ulid value)
        : base(value)
    {
    }

    public static Result<StorageAreaId> CreateFrom(Ulid value) => Result.FromValue(new StorageAreaId(value));
}
