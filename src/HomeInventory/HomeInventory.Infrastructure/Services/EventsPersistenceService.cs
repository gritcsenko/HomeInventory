﻿using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Framework;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Services;

internal class EventsPersistenceService(IDatabaseContext context) : IEventsPersistenceService
{
    private readonly IDatabaseContext _context = context;

    public ValueTask SaveEventsAsync(IHasDomainEvents entity, CancellationToken cancellationToken = default)
    {
        var events = entity.GetDomainEvents();
        var messages = events.Select(CreateMessage);
        _context.GetDbSet<OutboxMessage>().AddRange(messages);
        entity.ClearDomainEvents();
        return ValueTask.CompletedTask;
    }

    private static OutboxMessage CreateMessage(IDomainEvent domainEvent) =>
        new(domainEvent.Id, domainEvent.CreatedOn, domainEvent) { CreatedOn = domainEvent.CreatedOn };
}
