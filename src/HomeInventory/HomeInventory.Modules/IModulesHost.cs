using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;

namespace HomeInventory.Modules;

public interface IModulesHost
{
    Task<IRegisteredModules> AddServicesAsync(IServiceCollection services, IConfiguration configuration,
        IMetricsBuilder metrics, CancellationToken cancellationToken = default);
}
