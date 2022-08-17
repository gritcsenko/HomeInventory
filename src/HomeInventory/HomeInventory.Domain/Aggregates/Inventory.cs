using System.Collections.Immutable;
using HomeInventory.Domain.Entities;

namespace HomeInventory.Domain.Aggregates;

public class Inventory
{
    public Inventory(Stock stock)
    {
        Stock = stock;
    }

    public Stock Stock { get; init; }

    public IReadOnlyCollection<Material> Materials { get; init; } = ImmutableArray<Material>.Empty;
}

