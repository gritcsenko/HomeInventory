using HomeInventory.Domain.Entities;

namespace HomeInventory.Domain.Aggregates;

public class Inventory<T>
{
    public Stock<T> Stock { get; init; } = null!;

    public IReadOnlyCollection<Material<T>> Materials { get; init; } = Array.Empty<Material<T>>();
}

