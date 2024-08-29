using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Services;

internal class EventsPersistenceService(IDatabaseContext context) : IEventsPersistenceService
{
    private readonly IDatabaseContext _context = context;

    public async Task SaveEventsAsync(IHasDomainEvents entity, CancellationToken cancellationToken = default)
    {
        var events = entity.DomainEvents;
        var messages = events.Select(CreateMessage);
        await _context.GetDbSet<OutboxMessage>().AddRangeAsync(messages, cancellationToken);
        entity.ClearDomainEvents();
    }

    private static OutboxMessage CreateMessage(IDomainEvent domainEvent) =>
        new(domainEvent.Id, domainEvent.CreatedOn, domainEvent) { CreatedOn = domainEvent.CreatedOn };
}
