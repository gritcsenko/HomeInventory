using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class MaterialId : GuidIdentifierObject<MaterialId>, IIdentifierObject<MaterialId>
{
    internal MaterialId(Guid value)
        : base(value)
    {
    }

    public static explicit operator Guid(MaterialId obj) => obj.Value;
}
