using HomeInventory.Domain.Primitives.Events;

namespace HomeInventory.Application.Cqrs.DomainEvents;

public static class DomainEventNotification
{
    public static INotification Create(IDomainEvent domainEvent) =>
        (INotification)Activator.CreateInstance(typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent)!;
}

public class DomainEventNotification<TEvent>(TEvent domainEvent) : INotification
    where TEvent : IDomainEvent
{
    public TEvent DomainEvent { get; } = domainEvent;
}
