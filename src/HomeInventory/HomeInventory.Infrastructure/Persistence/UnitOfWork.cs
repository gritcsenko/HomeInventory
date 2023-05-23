using Ardalis.Specification;
using DotNext;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HomeInventory.Infrastructure.Persistence;

internal sealed class UnitOfWork : Disposable, IUnitOfWork
{
    private readonly DbContext _context;
    private readonly IDateTimeService _dateTimeService;
    private readonly IDisposable _attachedResource;
    private readonly bool _ownContext;
    private readonly ChangeTracker _changeTracker;
    private readonly DbSet<OutboxMessage> _messages;

    public UnitOfWork(DbContext context, IDateTimeService dateTimeService, IDisposable attachedResource)
        : this(context, dateTimeService, attachedResource, true)
    {
    }

    public UnitOfWork(DbContext context, IDateTimeService dateTimeService, IDisposable attachedResource, bool ownContext)
    {
        _context = context;
        _dateTimeService = dateTimeService;
        _attachedResource = attachedResource;
        _ownContext = ownContext;
        _changeTracker = _context.ChangeTracker;
        _messages = _context.Set<OutboxMessage>();
    }

    public DbContext DbContext => _context;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await ConvertDomainEventsToOutboxMessagesAsync(cancellationToken);

        UpdateAuditableEntities();

        await _context.SaveChangesAsync(cancellationToken);
    }

    ValueTask IAsyncDisposable.DisposeAsync() => DisposeAsync();

    protected override async ValueTask DisposeAsyncCore()
    {
        _attachedResource.Dispose();

        if (_ownContext)
        {
            await _context.DisposeAsync();
        }

        await base.DisposeAsyncCore();
    }

    private async Task ConvertDomainEventsToOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        var messages = _changeTracker
            .Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(root => root.GetAndClearEvents())
            .Select(CreateMessage);

        await _messages.AddRangeAsync(messages, cancellationToken);
    }

    private OutboxMessage CreateMessage(IDomainEvent domainEvent)
    {
        var id = Guid.NewGuid();
        return new OutboxMessage(id, _dateTimeService.UtcNow, domainEvent);
    }

    private void UpdateAuditableEntities()
    {
        foreach (var entry in _changeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(x => x.CreatedOn).CurrentValue = _dateTimeService.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Property(x => x.ModifiedOn).CurrentValue = _dateTimeService.UtcNow;
                    break;
            }
        }
    }
}
