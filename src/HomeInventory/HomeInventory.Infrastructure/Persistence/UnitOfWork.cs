using System.Text.Json;
using HomeInventory.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _context;
    private readonly IDateTimeService _dateTimeService;

    public UnitOfWork(DatabaseContext context, IDateTimeService dateTimeService)
    {
        _context = context;
        _dateTimeService = dateTimeService;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = _dateTimeService.Now.ToUniversalTime();

        await ConvertDomainEventsToOutboxMessages(now, cancellationToken);
        UpdateAuditableEntities(now);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task ConvertDomainEventsToOutboxMessages(DateTimeOffset now, CancellationToken cancellationToken)
    {
        var messages = _context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(root =>
            {
                var domainEvents = root.Events.ToArray();
                root.ClearEvents();
                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                now,
                domainEvent.GetType().Name,
                JsonSerializer.Serialize(domainEvent, new JsonSerializerOptions(JsonSerializerDefaults.Web))
            ))
            .ToArray();

        await _context.Set<OutboxMessage>().AddRangeAsync(messages, cancellationToken);
    }

    private void UpdateAuditableEntities(DateTimeOffset now)
    {
        foreach (var entry in _context.ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(x => x.CreatedOn).CurrentValue = now;
                    break;

                case EntityState.Modified:
                    entry.Property(x => x.ModifiedOn).CurrentValue = now;
                    break;
            }
        }
    }
}

public record OutboxMessage(Guid Id, DateTimeOffset OccurredOn, string Type, string Content);