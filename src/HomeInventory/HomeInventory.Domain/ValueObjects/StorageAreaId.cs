using DotNext;
using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.ValueObjects;

public sealed class StorageAreaId : UlidIdentifierObject<StorageAreaId>, IUlidBuildable<StorageAreaId>
{
    private StorageAreaId(Ulid value)
        : base(value)
    {
    }

    public static Optional<StorageAreaId> CreateFrom(Ulid value) => new StorageAreaId(value);
}
