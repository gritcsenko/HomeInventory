using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Domain.Primitives;

public interface IUnitOfWork : IAsyncDisposable
{
    DbContext DbContext { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
