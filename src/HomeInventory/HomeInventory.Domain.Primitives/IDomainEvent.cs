using MediatR;

namespace HomeInventory.Domain.Primitives;

public interface IDomainEvent : INotification
{
    IAggregateRoot Source { get; }
}
