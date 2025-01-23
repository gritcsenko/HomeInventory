using HomeInventory.Application;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HomeInventory.Infrastructure.Services;

internal sealed class PersistenceHealthCheck(DatabaseContext context) : BaseHealthCheck
{
    private readonly DatabaseContext _context = context;

    protected override IReadOnlyDictionary<string, object> ExceptionData => new Dictionary<string, object>
    {
        ["provider"] = ProviderName,
    };

    private string ProviderName => _context.Database.ProviderName ?? "Unknown";

    protected override async ValueTask<HealthCheckStatus> CheckHealthAsync(CancellationToken cancellationToken)
    {
        var database = _context.Database;

        var canConnect = await database.CanConnectAsync(cancellationToken);
        if (!canConnect)
        {
            return new()
            {
                IsFailed = true,
                Description = "Cannot connect to the database",
                Data = 
                {
                    ["provider"] = ProviderName,
                },
            };
        }

        if (database.IsRelational())
        {
            return await CheckRelationalStateAsync(database, cancellationToken);
        }

        return ReportHealthy();

    }

    private async Task<HealthCheckStatus> CheckRelationalStateAsync(DatabaseFacade database, CancellationToken cancellationToken)
    {
        var pendingMigrations = await database.GetPendingMigrationsAsync(cancellationToken);
        return pendingMigrations.Count() switch
        {
            0 => ReportHealthy(),
            var count => new()
            {
                IsFailed = true,
                Description = $"Database has {count} pending migrations",
                Data = 
                {
                    ["provider"] = ProviderName,
                    ["migrations.count"] = count,
                },
            }
        };
    }

    private HealthCheckStatus ReportHealthy() =>
        new()
        {
            Description = "Database is healthy",
            Data =
            {
                ["provider"] = ProviderName,
            },
        };
}
