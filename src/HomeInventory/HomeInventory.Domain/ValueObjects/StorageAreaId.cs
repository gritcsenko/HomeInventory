using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.ValueObjects;

public sealed class StorageAreaId(Ulid value) : UlidIdentifierObject<StorageAreaId>(value), IUlidBuildable<StorageAreaId>
{
    public static StorageAreaId CreateFrom(Ulid value) => new(value);
}
