using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public class MaterialId : GuidIdentifierObject<MaterialId>, IIdentifierObject<MaterialId>
{
    internal MaterialId(Guid value)
        : base(value)
    {
    }

    public static explicit operator Guid(MaterialId obj) => obj.Value;
}

public interface IMaterialIdFactory
{
    MaterialId CreateNew();
}

internal class MaterialIdFactory : ValueObjectFactory<MaterialId>, IMaterialIdFactory
{
    public MaterialId CreateNew() => new(Guid.NewGuid());
}
