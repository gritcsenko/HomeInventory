using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Entities;

public class Product
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = null!;
    public Amount Amount { get; init; } = null!;
}

