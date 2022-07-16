using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Entities;

public class Material<T>
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = null!;
    public Amount<T> Amount { get; init; } = null!;
}

