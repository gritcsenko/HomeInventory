using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Infrastructure.Persistence.Models;

internal class StorageAreaModel : IPersistentModel<StorageAreaId>
{
    public required StorageAreaId Id { get; init; }
    public required string Name { get; init; }
    public required List<ProductModel> Products { get; init; }
}
