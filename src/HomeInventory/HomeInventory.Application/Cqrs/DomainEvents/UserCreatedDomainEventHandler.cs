using HomeInventory.Domain.Aggregates;
using MediatR;

namespace HomeInventory.Application.Cqrs.DomainEvents;

internal class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
{
    public UserCreatedDomainEventHandler()
    {
    }

    public Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
