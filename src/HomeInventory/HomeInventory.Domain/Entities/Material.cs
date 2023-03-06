using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Entities;

public class Material : Entity<Material, MaterialId>
{
    public Material(MaterialId id)
        : base(id)
    {
    }

    public string Name { get; init; } = null!;

    public Amount Amount { get; init; } = null!;
}
