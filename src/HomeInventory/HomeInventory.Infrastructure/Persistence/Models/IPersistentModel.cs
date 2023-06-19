namespace HomeInventory.Infrastructure.Persistence.Models;

internal interface IPersistentModel : IPersistentModel<Ulid>
{
}

internal interface IPersistentModel<out TId>
    where TId : notnull, IEquatable<TId>
{
    TId Id { get; }
}