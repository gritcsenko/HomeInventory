using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace HomeInventory.Modules;

public sealed class ModulesHost(IReadOnlyCollection<IModule> modules)
{
    private readonly IReadOnlyCollection<IModule> _modules = modules;
    private readonly ModuleMetadataCollection _metadata = [];
    private IModule[] _availableModules = [];

    public async Task AddModulesAsync(IServiceCollection services, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        AddFeatures(services);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(configuration);
        AddFeatures(serviceCollection);

        await using var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        foreach (var module in _modules)
        {
            _metadata.Add(module);
        }

        var graph = await _metadata.CreateDependencyGraphAsync((m, _) => m.Module.Flag.IsEnabledAsync(featureManager), cancellationToken);
        var nodes = graph.KahnTopologicalSort();

        var sorted = nodes.Select(n => n.Value).ToArray();

        _availableModules = sorted.Select(m => m.Module).ToArray();
        var context = new ModuleServicesContext(services, configuration, featureManager, _availableModules);
        await Task.WhenAll(_availableModules.Select(async m => await m.AddServicesAsync(context, cancellationToken)));
    }

    public async Task BuildModulesAsync<TApp>(TApp app, CancellationToken cancellationToken = default)
        where TApp : IApplicationBuilder, IEndpointRouteBuilder
    {
        var context = new ModuleBuildContext<TApp>(app);
        await Task.WhenAll(_availableModules.Select(async m => await m.BuildAppAsync(context, cancellationToken)));
    }

    private static void AddFeatures(IServiceCollection services) => services.AddFeatureManagement();
}