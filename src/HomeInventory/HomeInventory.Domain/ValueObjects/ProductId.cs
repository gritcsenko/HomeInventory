using DotNext;
using HomeInventory.Domain.Primitives.Ids;
using Visus.Cuid;

namespace HomeInventory.Domain.ValueObjects;

public sealed class ProductId : CuidIdentifierObject<ProductId>, ICuidBuildable<ProductId>
{
    private ProductId(Cuid value)
        : base(value)
    {
    }

    public static Result<ProductId> CreateFrom(Cuid value) => Result.FromValue(new ProductId(value));
}
