using System.Diagnostics.CodeAnalysis;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace HomeInventory.Modules;

[SuppressMessage("Design", "CA1067:Override Object.Equals(object) when implementing IEquatable<T>", Justification = "Implements IEquatable<T>")]
public sealed record ModuleServicesContext(IServiceCollection Services, IConfiguration Configuration, IFeatureManager FeatureManager, IReadOnlyCollection<IModule> Modules) : IModuleServicesContext;
