using Visus.Cuid;

namespace HomeInventory.Infrastructure.Persistence.Models;

public interface IPersistentModel : IPersistentModel<Cuid>
{
}

public interface IPersistentModel<out TId>
    where TId : notnull, IEquatable<TId>
{
    TId Id { get; }
}