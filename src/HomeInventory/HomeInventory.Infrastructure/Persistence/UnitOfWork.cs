using DotNext;
using HomeInventory.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal sealed class UnitOfWork : Disposable, IUnitOfWork
{
    private readonly DbContext _context;
    private readonly IDisposable _attachedResource;

    public UnitOfWork(DbContext context, IDisposable attachedResource)
    {
        _context = context;
        _attachedResource = attachedResource;
    }

    public DbContext DbContext => _context;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) => await _context.SaveChangesAsync(cancellationToken);

    ValueTask IAsyncDisposable.DisposeAsync() => DisposeAsync();

    protected override async ValueTask DisposeAsyncCore()
    {
        _attachedResource.Dispose();

        await _context.DisposeAsync();

        await base.DisposeAsyncCore();
    }
}
