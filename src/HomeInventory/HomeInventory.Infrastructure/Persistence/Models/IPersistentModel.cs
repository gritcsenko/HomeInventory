using HomeInventory.Domain.Primitives;

namespace HomeInventory.Infrastructure.Persistence.Models;

internal interface IPersistentModel
{
    Guid Id { get; }
}

internal interface IPersistentModel<out TId>
    where TId : GuidIdentifierObject<TId>
{
    TId Id { get; }
}