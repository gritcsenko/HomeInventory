using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections;
using System.Reflection;
using Microsoft.FeatureManagement;

namespace HomeInventory.Modules;

public class ModulesCollection : IReadOnlyCollection<IModule>
{
    private readonly List<IModule> _modules = [];

    public int Count => _modules.Count;

    public void Add(IModule module)
    {
        _modules.Add(module);
        if (module is IAttachableModule attachable)
        {
            attachable.OnAttached(this);
        }
    }

    public IEnumerator<IModule> GetEnumerator() => _modules.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public sealed class ModulesHost(IReadOnlyCollection<IModule> modules)
{
    private readonly IReadOnlyCollection<IModule> _modules = modules;
    private readonly ModuleMetadataCollection _metadata = [];
    private IModule[] _availableModules = [];

    public async Task InjectToAsync(IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        AddFeatures(services);

        var configuration = builder.Configuration;

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(configuration);
        AddFeatures(serviceCollection);

        using var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        foreach (var module in _modules)
        {
            var moduleType = module.GetType();
            var feature = FeatureFlag.Create(moduleType.FullName ?? moduleType.Name);
            if (await feature.IsEnabledAsync(featureManager)) 
            {
                _metadata.Add(module);
            }
        }

        var graph = _metadata.CreateDependencyGraph();
        var result = graph.DeepFirstTraversalKahnTopologicalSort();
        if (!result.IsAcrylic)
        {
            var cycles = result.DetectedCycles;
        }

        var sorted = result.Sorted.Select(n => n.Value).ToArray();

        _availableModules = sorted.Select(m => m.Module).ToArray();
        await Task.WhenAll(_modules.Select(async m => await m.AddServicesAsync(services, configuration, featureManager, _availableModules)));
    }

    public async Task BuildIntoAsync(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder)
    {
        await Task.WhenAll(_availableModules.Select(async m => await m.BuildAppAsync(applicationBuilder, endpointRouteBuilder)));
    }

    private static void AddFeatures(IServiceCollection services)
    {
        services.AddFeatureManagement();
    }
}

public interface IModuleSorter
{
    IEnumerable<ModuleMetadata> Sort(IReadOnlyCollection<ModuleMetadata> modules);
}

public sealed class ModuleMetadataCollection : IReadOnlyCollection<ModuleMetadata>
{
    private readonly List<ModuleMetadata> _metadata = [];

    public int Count => _metadata.Count;

    public void Add(IModule module) 
    {
        _metadata.Add(new ModuleMetadata(module, this));
    }

    public IEnumerator<ModuleMetadata> GetEnumerator() => _metadata.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public DirectedAcyclicGraph<ModuleMetadata, Type> CreateDependencyGraph()
    {

    }
}

public sealed class ModuleMetadata(IModule module, ModuleMetadataCollection container)
{
    private readonly ModuleMetadataCollection _container = container;

    public Type ModuleType { get; } = module.GetType();

    public IModule Module { get; } = module;

    public IEnumerable<Option<ModuleMetadata>> GetDependencies() => Module.Dependencies.Select(d => _container.Find(m => m.ModuleType == d));
}
