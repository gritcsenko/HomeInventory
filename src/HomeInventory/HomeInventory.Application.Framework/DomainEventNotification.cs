using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Framework;

public static class DomainEventNotification
{
    public static IDomainEventNotification Create(IDomainEvent domainEvent) =>
        (IDomainEventNotification)Activator.CreateInstance(typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent)!;
}

public class DomainEventNotification<TEvent>(TEvent domainEvent) : IDomainEventNotification
    where TEvent : IDomainEvent
{
    public TEvent DomainEvent { get; } = domainEvent;

    IDomainEvent IDomainEventNotification.DomainEvent { get; } = domainEvent;
}
