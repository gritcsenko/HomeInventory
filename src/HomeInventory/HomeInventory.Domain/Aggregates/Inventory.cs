using HomeInventory.Domain.Entities;

namespace HomeInventory.Domain.Aggregates;

public class Inventory
{
    public Stock Stock { get; init; } = null!;

    public IReadOnlyCollection<Material> Materials { get; init; } = Array.Empty<Material>();
}

