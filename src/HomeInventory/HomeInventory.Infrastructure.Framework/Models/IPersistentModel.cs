namespace HomeInventory.Infrastructure.Framework.Models;

public interface IPersistentModel : IPersistentModel<Ulid>
{
}

public interface IPersistentModel<out TId>
    where TId : notnull, IEquatable<TId>
{
    TId Id { get; }
}