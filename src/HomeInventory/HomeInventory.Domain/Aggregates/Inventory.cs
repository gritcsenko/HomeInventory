using HomeInventory.Domain.Entities;

namespace HomeInventory.Domain.Aggregates;

public class Inventory : Stock
{
    public IReadOnlyCollection<Material> Materials { get; init; } = [];
}

