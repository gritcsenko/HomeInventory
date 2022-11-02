using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Aggregates;

public class StorageArea : AggregateRoot<StorageArea, StorageAreaId>
{
    public StorageArea(StorageAreaId id)
        : base(id)
    {
    }

    public StorageAreaName Name { get; init; } = null!;
}