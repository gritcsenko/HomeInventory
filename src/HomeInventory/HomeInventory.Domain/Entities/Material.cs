using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Entities;

public class Material(MaterialId id) : Entity<Material, MaterialId>(id)
{
    public required string Name { get; init; }

    public required Amount Amount { get; init; }
}
