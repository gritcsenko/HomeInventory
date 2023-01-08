namespace HomeInventory.Infrastructure.Persistence.Models;

internal class ProductModel : IPersistentModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required ProductAmountModel Amount { get; init; }
}
