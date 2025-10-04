using HomeInventory.Domain.Entities;

namespace HomeInventory.Domain.Aggregates;

public class Stock
{
    public IReadOnlyCollection<Product> Products { get; init; } = [];
}
