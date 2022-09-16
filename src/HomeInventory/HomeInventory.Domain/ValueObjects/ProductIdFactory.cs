using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

internal sealed class ProductIdFactory : ValueObjectFactory<ProductId>, IProductIdFactory
{
    public ProductId CreateNew() => new(Guid.NewGuid());
}
