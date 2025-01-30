using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace HomeInventory.Modules.Interfaces;

public interface IModuleServicesContext
{
    IServiceCollection Services { get; }
    IConfiguration Configuration { get; }
    IFeatureManager FeatureManager { get; }
    IReadOnlyCollection<IModule> Modules { get; }
}