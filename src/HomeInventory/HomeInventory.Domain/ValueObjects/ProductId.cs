using DotNext;
using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.ValueObjects;

public sealed class ProductId : UlidIdentifierObject<ProductId>, IUlidBuildable<ProductId>
{
    private ProductId(Ulid value)
        : base(value)
    {
    }

    public static Optional<ProductId> CreateFrom(Ulid value) => new ProductId(value);
}
