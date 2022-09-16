using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;
public sealed class ProductId : GuidIdentifierObject<ProductId>
{
    internal ProductId(Guid value)
        : base(value)
    {
    }
}
