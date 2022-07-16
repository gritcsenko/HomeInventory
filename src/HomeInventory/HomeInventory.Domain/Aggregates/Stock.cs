using HomeInventory.Domain.Entities;

namespace HomeInventory.Domain.Aggregates;

public class Stock<T>
{
    public IReadOnlyCollection<Product<T>> Products { get; init; } = Array.Empty<Product<T>>();
}
