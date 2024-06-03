﻿using HomeInventory.Domain.Primitives.Messages;
using Visus.Cuid;

namespace HomeInventory.Application.Cqrs.DomainEvents;

public static class DomainEventNotification
{
    public static IMessage CreateDomainNotification(this IMessageHub hub, IDomainEvent domainEvent) =>
        (IMessage)Activator.CreateInstance(typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent)!;
}

public class DomainEventNotification<TEvent>(TEvent domainEvent) : IMessage
    where TEvent : IDomainEvent
{
    public TEvent DomainEvent { get; } = domainEvent;
    public Cuid Id => DomainEvent.Id;
    public DateTimeOffset CreatedOn => DomainEvent.CreatedOn;
}
