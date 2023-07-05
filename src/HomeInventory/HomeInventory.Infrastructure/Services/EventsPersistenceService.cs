using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Services;

internal class EventsPersistenceService : IEventsPersistenceService
{
    private readonly IDatabaseContext _context;

    public EventsPersistenceService(IDatabaseContext context)
    {
        _context = context;
    }

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
