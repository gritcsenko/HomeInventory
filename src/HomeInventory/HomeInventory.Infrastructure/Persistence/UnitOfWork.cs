using DotNext;
using HomeInventory.Domain.Primitives;
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
    }

    public DbContext DbContext => _context;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
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
