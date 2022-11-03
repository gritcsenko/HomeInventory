using HomeInventory.Domain.Primitives;
using MediatR;

namespace HomeInventory.Domain.Events;

public interface IDomainEvent : INotification
{
    IAggregateRoot Source { get; }
}
