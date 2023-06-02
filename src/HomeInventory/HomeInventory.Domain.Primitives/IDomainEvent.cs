﻿namespace HomeInventory.Domain.Primitives;

public interface IDomainEvent : IEvent, IHaveCreationAudit
{
    IAggregateRoot Source { get; }
}
