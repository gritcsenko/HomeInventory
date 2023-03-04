﻿namespace HomeInventory.Domain.Primitives;

public abstract class AggregateRoot<TAggregate, TIdentity> : Entity<TAggregate, TIdentity>, IAggregateRoot
    where TIdentity : IIdentifierObject<TIdentity>
    where TAggregate : AggregateRoot<TAggregate, TIdentity>
{
    private readonly List<IDomainEvent> _events = new();

    protected AggregateRoot(TIdentity id)
        : base(id)
    {
    }

    public IReadOnlyCollection<IDomainEvent> GetAndClearEvents()
    {
        var events = _events.ToArray();
        _events.Clear();
        return events;
    }

    protected void AddEvent(IDomainEvent @event) => _events.Add(@event);
}