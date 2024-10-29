using HomeInventory.Application;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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
                Data = {
                    ["provider"] = ProviderName,
                },
            };
        }

        if (database.IsRelational())
        {
            var pendingMigrations = await database.GetPendingMigrationsAsync(cancellationToken);
            var count = pendingMigrations.Count();
            if (count != 0)
            {
                return new()
                {
                    IsFailed = true,
                    Description = $"Database has {count} pending migrations",
                    Data = {
                        ["provider"] = ProviderName,
                        ["migrations.count"] = count,
                    },
                };
            }
        }

        return new()
        {
            Description = "Database is healthy",
            Data = {
                ["provider"] = ProviderName,
            },
        };
    }
}
