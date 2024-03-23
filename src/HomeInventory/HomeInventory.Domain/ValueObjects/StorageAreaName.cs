using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class StorageAreaName(string value) : ValueObject<StorageAreaName>(value)
{
    public string Value { get; } = value;
}
