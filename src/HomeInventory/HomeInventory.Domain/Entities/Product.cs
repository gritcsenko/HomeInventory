using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Entities;

public class Product(ProductId id) : Entity<Product, ProductId>(id)
{
    public required string Name { get; init; }

    public required Amount Amount { get; init; }
}
