using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;
public sealed class ProductId : UlidIdentifierObject<ProductId>
{
    internal ProductId(Ulid value)
        : base(value)
    {
    }
}
