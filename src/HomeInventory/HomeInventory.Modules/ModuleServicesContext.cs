using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace HomeInventory.Modules;

public sealed record ModuleServicesContext(IServiceCollection Services, IConfiguration Configuration, IFeatureManager FeatureManager, IReadOnlyCollection<IModule> Modules) : IModuleServicesContext;
