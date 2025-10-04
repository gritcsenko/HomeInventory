using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.ValueObjects;

public sealed class ProductId(Ulid value) : UlidIdentifierObject<ProductId>(value), IUlidBuildable<ProductId>
{
    public static ProductId CreateFrom(Ulid value) => new(value);
}
