using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace HomeInventory.Modules.Interfaces;

public sealed record class ModuleServicesContext(IServiceCollection Services, IConfiguration Configuration, IFeatureManager FeatureManager, IReadOnlyCollection<IModule> Modules);
