using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HomeInventory.Infrastructure.Services;

internal class PersistenceHealthCheck : IHealthCheck
{
    private readonly IDbContextFactory<DatabaseContext> _factory;

    public PersistenceHealthCheck(IDbContextFactory<DatabaseContext> contextFactory) => _factory = contextFactory;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _factory.CreateDbContextAsync(cancellationToken);
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
        if (canConnect)
        {
            return HealthCheckResult.Healthy();
        }

        return HealthCheckResult.Unhealthy();
    }
}
