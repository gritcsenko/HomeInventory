using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class StorageAreaName : ValueObject<StorageAreaName>
{
    internal StorageAreaName(string value)
        : base(value)
    {
        Value = value;
    }

    public string Value { get; }
}
