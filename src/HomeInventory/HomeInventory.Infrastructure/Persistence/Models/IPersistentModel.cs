namespace HomeInventory.Infrastructure.Persistence.Models;

internal interface IPersistentModel
{
    Guid Id { get; init; }
}
