using Visus.Cuid;

namespace HomeInventory.Infrastructure.Persistence.Models;

internal class ProductModel : IPersistentModel
{
    public required Cuid Id { get; init; }
    public required string Name { get; init; }
    public required ProductAmountModel Amount { get; init; }
}
