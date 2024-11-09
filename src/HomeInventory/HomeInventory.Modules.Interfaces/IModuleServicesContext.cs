using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace HomeInventory.Modules.Interfaces;

public interface IModuleServicesContext
{
    IServiceCollection Services { get; init; }
    IConfiguration Configuration { get; init; }
    IFeatureManager FeatureManager { get; init; }
    IReadOnlyCollection<IModule> Modules { get; init; }
}