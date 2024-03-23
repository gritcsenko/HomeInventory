using DotNext;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class ProductId : UlidIdentifierObject<ProductId>, IUlidBuildable<ProductId>
{
    private ProductId(Ulid value)
        : base(value)
    {
    }

    public static Result<ProductId> CreateFrom(Ulid value) => Result.FromValue(new ProductId(value));
}
