using System.Text.Json;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal sealed class UnitOfWork : AsyncDisposable, IUnitOfWork
{
    private readonly DbContext _context;
    private readonly IDateTimeService _dateTimeService;
    private readonly bool _ownContext;

    public UnitOfWork(DbContext context, IDateTimeService dateTimeService, bool ownContext = true)
    {
        _context = context;
        _dateTimeService = dateTimeService;
        _ownContext = ownContext;
    }

    public DbContext DbContext => _context;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = _dateTimeService.Now.ToUniversalTime();

        await ConvertDomainEventsToOutboxMessages(now, cancellationToken);
        UpdateAuditableEntities(now);

        await _context.SaveChangesAsync(cancellationToken);
    }

    protected override async ValueTask InternalDisposeAsync()
    {
        if (_ownContext)
        {
            await _context.DisposeAsync();
        }
    }

    private async Task ConvertDomainEventsToOutboxMessages(DateTimeOffset now, CancellationToken cancellationToken)
    {
        var messages = _context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(root =>
            {
                var domainEvents = root.GetEvents();
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
