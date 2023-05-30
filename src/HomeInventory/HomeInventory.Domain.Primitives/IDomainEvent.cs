using MediatR;

namespace HomeInventory.Domain.Primitives;

public interface IDomainEvent : INotification
{
    Guid Id { get; }

    DateTimeOffset Created { get; }

    IAggregateRoot Source { get; }
}
