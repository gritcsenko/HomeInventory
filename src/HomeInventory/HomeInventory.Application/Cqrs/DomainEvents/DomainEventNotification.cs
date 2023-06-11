using HomeInventory.Domain.Primitives;
using MediatR;

namespace HomeInventory.Application.Cqrs.DomainEvents;

public class DomainEventNotification<TEvent> : INotification
    where TEvent : IDomainEvent
{
    public DomainEventNotification(TEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }

    public TEvent DomainEvent { get; }
}
