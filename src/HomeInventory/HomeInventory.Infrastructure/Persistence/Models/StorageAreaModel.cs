namespace HomeInventory.Infrastructure.Persistence.Models;

internal class StorageAreaModel : IPersistentModel
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required List<ProductModel> Products { get; init; }
}
