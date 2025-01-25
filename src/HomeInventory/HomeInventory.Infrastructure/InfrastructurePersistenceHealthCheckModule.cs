using HomeInventory.Application;
using HomeInventory.Infrastructure.Services;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HomeInventory.Infrastructure;

public sealed class InfrastructurePersistenceHealthCheckModule : BaseModule
{
    public override async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default)
    {
        await base.AddServicesAsync(context, cancellationToken);

        context.Services
            .AddHealthChecks()
            .AddCheck<PersistenceHealthCheck>("Persistence", HealthStatus.Unhealthy, [HealthCheckTags.Ready]);
    }
}
