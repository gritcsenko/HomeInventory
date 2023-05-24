namespace HomeInventory.Infrastructure.Persistence.Models;

internal interface IPersistentModel : IPersistentModel<Guid>
{
}

internal interface IPersistentModel<out TId>
    where TId : notnull, IEquatable<TId>
{
    TId Id { get; }
}