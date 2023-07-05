﻿using HomeInventory.Domain.Events;

namespace HomeInventory.Application.Cqrs.DomainEvents;

internal class UserCreatedDomainEventHandler : INotificationHandler<DomainEventNotification<UserCreatedDomainEvent>>
{
    public Task Handle(DomainEventNotification<UserCreatedDomainEvent> notification, CancellationToken cancellationToken) => Task.CompletedTask;
}
