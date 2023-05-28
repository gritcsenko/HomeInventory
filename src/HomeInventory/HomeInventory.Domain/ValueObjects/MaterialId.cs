using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class MaterialId : GuidIdentifierObject<MaterialId>
{
    internal MaterialId(Guid value)
        : base(value)
    {
    }
}
