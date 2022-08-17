using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Entities;

public class Product : Entity<Product, ProductId>
{
    public Product(ProductId id)
        : base(id)
    {
    }

    public string Name { get; init; } = null!;

    public Amount Amount { get; init; } = null!;
}

