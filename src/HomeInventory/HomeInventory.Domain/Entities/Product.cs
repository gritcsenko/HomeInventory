using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Entities;

public class Product : Entity<Product, ProductId>
{
    public Product(ProductId id)
        : base(id)
    {
    }

    public required string Name { get; init; }

    public required Amount Amount { get; init; }
}
