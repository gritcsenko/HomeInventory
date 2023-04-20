using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

internal sealed class MaterialIdFactory : ValueObjectFactory<MaterialId>, IMaterialIdFactory
{
    public MaterialId CreateNew() => new(Guid.NewGuid());
}
