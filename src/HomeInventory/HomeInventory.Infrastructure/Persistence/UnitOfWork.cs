using DotNext;
using HomeInventory.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal sealed class UnitOfWork : Disposable, IUnitOfWork
{
    private readonly DbContext _context;
    private readonly IDisposable _attachedResource;
    private readonly bool _ownContext;

    public UnitOfWork(DbContext context, IDisposable attachedResource)
        : this(context, attachedResource, true)
    {
    }

    public UnitOfWork(DbContext context, IDisposable attachedResource, bool ownContext)
    {
        _context = context;
        _attachedResource = attachedResource;
        _ownContext = ownContext;
    }

    public DbContext DbContext => _context;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
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
}
