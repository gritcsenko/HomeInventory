namespace HomeInventory.Infrastructure.Persistence;

internal interface IDbContext : IDisposable, IAsyncDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

