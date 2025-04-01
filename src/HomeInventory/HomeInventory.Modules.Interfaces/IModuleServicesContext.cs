using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.FeatureManagement;

namespace HomeInventory.Modules.Interfaces;

public interface IModuleServicesContext
{
    IServiceCollection Services { get; }

    IConfiguration Configuration { get; }
    
    IMetricsBuilder Metrics { get; } 
    
    IFeatureManager FeatureManager { get; }
    
    IReadOnlyCollection<IModule> Modules { get; }
}