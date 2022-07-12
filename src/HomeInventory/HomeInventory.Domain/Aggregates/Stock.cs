using HomeInventory.Domain.Entities;

namespace HomeInventory.Domain.Aggregates;

public class Stock
{
    public IReadOnlyCollection<Goods> Goods { get; init; } = Array.Empty<Goods>();
}
