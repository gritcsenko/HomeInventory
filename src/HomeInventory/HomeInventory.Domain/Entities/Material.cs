using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Entities;

public class Material : Entity<Material, MaterialId>
{
    public Material(MaterialId id)
        : base(id)
    {
    }

    public required string Name { get; init; }

    public required Amount Amount { get; init; }
}
