using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Framework;

public interface IDomainEventNotification
{
    IDomainEvent DomainEvent { get; }
}