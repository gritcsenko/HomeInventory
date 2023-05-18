using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HomeInventory.Infrastructure.Persistence;

internal sealed class UnitOfWork : CompositeAsyncDisposable, IUnitOfWork
{
    private readonly DbContext _context;
    private readonly IDateTimeService _dateTimeService;
    private readonly ChangeTracker _changeTracker;
    private readonly DbSet<OutboxMessage> _messages;

    public UnitOfWork(DbContext context, IDateTimeService dateTimeService, bool ownContext = true)
        : base(context)
    {
        _context = context;
        _dateTimeService = dateTimeService;
        _changeTracker = _context.ChangeTracker;
        _messages = _context.Set<OutboxMessage>();
        if (!ownContext)
        {
            Remove(context);
        }
    }

    public DbContext DbContext => _context;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = _dateTimeService.UtcNow;

        await ConvertDomainEventsToOutboxMessagesAsync(now, cancellationToken);

        UpdateAuditableEntities(now);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task ConvertDomainEventsToOutboxMessagesAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        var messages = _changeTracker
            .Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(root => root.GetAndClearEvents())
            .Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                now,
                domainEvent
            ));

        await _messages.AddRangeAsync(messages, cancellationToken);
    }

    private void UpdateAuditableEntities(DateTimeOffset now)
    {
        foreach (var entry in _changeTracker.Entries<IAuditableEntity>())
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
