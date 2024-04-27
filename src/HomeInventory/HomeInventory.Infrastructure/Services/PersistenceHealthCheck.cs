using DotNext.Threading.Tasks;
using HomeInventory.Application;
using HomeInventory.Core;
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
            return new HealthCheckStatus
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
            var pendingMigrations = await database.GetPendingMigrationsAsync(cancellationToken).Convert(x => x.ToReadOnly());
            if (pendingMigrations.Count != 0)
            {
                return new HealthCheckStatus
                {
                    IsFailed = true,
                    Description = $"Database has {pendingMigrations.Count} pending migrations",
                    Data = {
                        ["provider"] = ProviderName,
                        ["migrations.count"] = pendingMigrations.Count,
                    },
                };
            }
        }

        return new HealthCheckStatus
        {
            Description = "Database is healthy",
            Data = {
                ["provider"] = ProviderName,
            },
        };
    }
}
