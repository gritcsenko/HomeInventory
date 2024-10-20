using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace HomeInventory.Modules;

public class ModulesCollection : IReadOnlyCollection<IModule>
{
    private readonly System.Collections.Generic.HashSet<IModule> _modules = new(new ModuleEqualityComparer());

    public int Count => _modules.Count;

    public void Add(IModule module)
    {
        _modules.Add(module);
    }

    public IEnumerator<IModule> GetEnumerator() => _modules.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private sealed class ModuleEqualityComparer : IEqualityComparer<IModule>
    {
        public bool Equals(IModule? x, IModule? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.GetType().Equals(y.GetType());
        }

        public int GetHashCode([DisallowNull] IModule obj) => obj.GetType().GetHashCode();
    }
}

public sealed class ModulesHost(IReadOnlyCollection<IModule> modules)
{
    private readonly IReadOnlyCollection<IModule> _modules = modules;
    private readonly ModuleMetadataCollection _metadata = [];
    private IModule[] _availableModules = [];

    public async Task InjectToAsync(IServiceCollection services, IConfiguration configuration)
    {
        AddFeatures(services);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(configuration);
        AddFeatures(serviceCollection);

        using var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        foreach (var module in _modules)
        {
            _metadata.Add(module);
        }

        var graph = await _metadata.CreateDependencyGraph(m => m.Flag.IsEnabledAsync(featureManager));
        var nodes = graph.KahnTopologicalSort();

        var sorted = nodes.Select(n => n.Value).ToArray();

        _availableModules = sorted.Select(m => m.Module).ToArray();
        var context = new ModuleServicesContext(services, configuration, featureManager, _availableModules);
        await Task.WhenAll(_availableModules.Select(async m => await m.AddServicesAsync(context)));
    }

    public async Task BuildIntoAsync(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder)
    {
        var context = new ModuleBuildContext(applicationBuilder, endpointRouteBuilder);
        await Task.WhenAll(_availableModules.Select(async m => await m.BuildAppAsync(context)));
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
        _metadata.Add(new ModuleMetadata(module));
    }

    public IEnumerator<ModuleMetadata> GetEnumerator() => _metadata.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public async Task<DirectedAcyclicGraph<ModuleMetadata, Type>> CreateDependencyGraph(Func<ModuleMetadata, Task<bool>> canLoadAsync)
    {
        var graph = new DirectedAcyclicGraph<ModuleMetadata, Type>();
        var loadable = await GetLoadableModules(canLoadAsync);

        foreach (var meta in loadable)
        {
            var source = graph.GetOrAdd(meta, static (n, v) => n.Value == v);
            foreach (var reference in meta.GetDependencies(loadable))
            {
                reference.Do(r =>
                {
                    var target = graph.GetOrAdd(r, static (n, v) => n.Value == v);
                    graph.AddEdge(source, target, r.ModuleType);
                });
            }
        }

        return graph;
    }

    private async Task<List<ModuleMetadata>> GetLoadableModules(Func<ModuleMetadata, Task<bool>> canLoadAsync)
    {
        var loadable = _metadata.ToList();
        var canLoadCache = new Dictionary<Type, bool>();
        while (true)
        {
            var count = loadable.Count;
            foreach (var meta in loadable.Memo())
            {
                if (!await CanLoadAsync(meta))
                {
                    loadable.Remove(meta);
                    continue;
                }

                foreach (var optional in meta.GetDependencies(_metadata))
                {
                    if (optional.IsNone || !await CanLoadAsync((ModuleMetadata)optional))
                    {
                        loadable.Remove(meta);
                        break;
                    }
                }
            }

            if (loadable.Count == count)
            {
                return loadable;
            }
        }

        ValueTask<bool> CanLoadAsync(ModuleMetadata metadata)
        {
            return canLoadCache.GetOrAddAsync(metadata.ModuleType, _ => canLoadAsync(metadata));
        }
    }
}

public sealed class ModuleMetadata
{
    public ModuleMetadata(IModule module)
    {
        ModuleType = module.GetType();
        Module = module;
        Flag = FeatureFlag.Create(ModuleType.FullName ?? ModuleType.Name);
    }

    public Type ModuleType { get; }

    public IModule Module { get; }

    public IFeatureFlag Flag { get; }

    public IEnumerable<Option<ModuleMetadata>> GetDependencies(IReadOnlyCollection<ModuleMetadata> container) =>
        Module.Dependencies.Select(d => container.Find(m => m.ModuleType == d));
}
