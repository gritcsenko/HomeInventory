using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class MaterialId : UlidIdentifierObject<MaterialId>
{
    internal MaterialId(Ulid value)
        : base(value)
    {
    }
}
