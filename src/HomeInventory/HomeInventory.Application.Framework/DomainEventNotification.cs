using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Framework;

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
