using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Cqrs.DomainEvents;

public static class DomainEventNotification
{
    public static INotification Create(IDomainEvent domainEvent) =>
        (INotification)Activator.CreateInstance(typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent)!;
}

public class DomainEventNotification<TEvent> : INotification
    where TEvent : IDomainEvent
{
    public DomainEventNotification(TEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }

    public TEvent DomainEvent { get; }
}
