﻿using HomeInventory.Domain.Aggregates;
using MediatR;

namespace HomeInventory.Application.Cqrs.DomainEvents;

internal class UserCreatedDomainEventHandler : INotificationHandler<DomainEventNotification<UserCreatedDomainEvent>>
{
    public UserCreatedDomainEventHandler()
    {
    }

    public async Task Handle(DomainEventNotification<UserCreatedDomainEvent> notification, CancellationToken cancellationToken)
    {
        await ValueTask.CompletedTask;
    }
}