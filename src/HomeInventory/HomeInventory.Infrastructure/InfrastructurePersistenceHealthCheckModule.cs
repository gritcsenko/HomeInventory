using HomeInventory.Application;
using HomeInventory.Infrastructure.Services;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection;

public sealed class InfrastructurePersistenceHealthCheckModule : BaseModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
             .AddCheck<PersistenceHealthCheck>("Persistence", HealthStatus.Unhealthy, [HealthCheckTags.Ready]);
    }
}
