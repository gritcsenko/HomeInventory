using DotNext;
using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.ValueObjects;

public sealed class MaterialId : UlidIdentifierObject<MaterialId>, IUlidBuildable<MaterialId>
{
    private MaterialId(Ulid value)
        : base(value)
    {
    }

    public static Result<MaterialId> CreateFrom(Ulid value) => Result.FromValue(new MaterialId(value));
}
