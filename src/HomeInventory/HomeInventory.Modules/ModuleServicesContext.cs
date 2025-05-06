using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.FeatureManagement;

namespace HomeInventory.Modules;

public sealed record ModuleServicesContext(
    IServiceCollection Services,
    IConfiguration Configuration,
    IMetricsBuilder Metrics,
    IFeatureManager FeatureManager,
    IReadOnlyCollection<IModule> Modules) : IModuleServicesContext
{
    internal Task AddServicesAsync(CancellationToken cancellationToken = default) => Task.WhenAll(Modules.Select(async m => await m.AddServicesAsync(this, cancellationToken)));
}
