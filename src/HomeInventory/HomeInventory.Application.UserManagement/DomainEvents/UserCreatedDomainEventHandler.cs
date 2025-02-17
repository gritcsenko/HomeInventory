using HomeInventory.Application.Framework;
using HomeInventory.Domain.UserManagement.Events;

namespace HomeInventory.Application.UserManagement.DomainEvents;

internal sealed class UserCreatedDomainEventHandler : INotificationHandler<DomainEventNotification<UserCreatedDomainEvent>>
{
    public Task Handle(DomainEventNotification<UserCreatedDomainEvent> notification, CancellationToken cancellationToken) => Task.CompletedTask;
}
