using DotNext;
using HomeInventory.Domain.Primitives.Ids;
using Visus.Cuid;

namespace HomeInventory.Domain.ValueObjects;

public sealed class MaterialId : CuidIdentifierObject<MaterialId>, ICuidBuildable<MaterialId>
{
    private MaterialId(Cuid value)
        : base(value)
    {
    }

    public static Result<MaterialId> CreateFrom(Cuid value) => Result.FromValue(new MaterialId(value));
}
