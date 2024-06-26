using HomeInventory.Domain.Events;
using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Application.Cqrs.DomainEvents;

internal sealed class UserCreatedDomainEventHandler : IMessageHandler<DomainEventNotification<UserCreatedDomainEvent>>
{
    public Task HandleAsync(IMessageHub hub, DomainEventNotification<UserCreatedDomainEvent> message, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
