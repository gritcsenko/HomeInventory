using HomeInventory.Application;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Services;

internal class PersistenceHealthCheck : BaseHealthCheck
{
    private readonly IDbContextFactory<DatabaseContext> _factory;

    public PersistenceHealthCheck(IDbContextFactory<DatabaseContext> contextFactory) => _factory = contextFactory;

    protected override async ValueTask<bool> IsHealthyAsync(CancellationToken cancellationToken)
    {
        await using var dbContext = await _factory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Database.CanConnectAsync(cancellationToken)
            && !(await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any();
    }
}
