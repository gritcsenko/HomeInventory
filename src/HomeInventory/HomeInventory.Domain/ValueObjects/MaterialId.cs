using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.ValueObjects;

public sealed class MaterialId(Ulid value) : UlidIdentifierObject<MaterialId>(value), IUlidBuildable<MaterialId>
{
    public static MaterialId CreateFrom(Ulid value) => new(value);
}
