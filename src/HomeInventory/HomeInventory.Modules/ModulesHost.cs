using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.FeatureManagement;

namespace HomeInventory.Modules;

public sealed class ModulesHost(IReadOnlyCollection<IModule> modules) : IModulesHost
{
    private readonly ModuleMetadataCollection _metadata = new(modules);
    
    public static IModulesHost Create(IReadOnlyCollection<IModule> modules) => new ModulesHost(modules);
    
    public async Task<IRegisteredModules> AddServicesAsync(IServiceCollection services, IConfiguration configuration, IMetricsBuilder metrics, CancellationToken cancellationToken = default)
    {
        services.AddFeatureManagement(configuration);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddFeatureManagement(configuration);
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        var graph = await _metadata.CreateDependencyGraphAsync((m, _) => m.Module.Flag.IsEnabledAsync(featureManager), cancellationToken);
        var sortedNodes = graph.KahnTopologicalSort();
        var sortedMetadata = sortedNodes.Select(n => n.Value);
        var availableModules = sortedMetadata.Select(m => m.Module).ToArray();

        var context = new ModuleServicesContext(services, configuration, metrics, featureManager, availableModules);
        await context.AddServicesAsync(cancellationToken);
        return new RegisteredModules(context);
    }

    private sealed class RegisteredModules(IModuleServicesContext context) : IRegisteredModules
    {
        private readonly IModuleServicesContext _context = context;

        public Task BuildApplicationAsync<TApp>(TApp app, CancellationToken cancellationToken = default)
            where TApp : IApplicationBuilder, IEndpointRouteBuilder =>
            new ModuleBuildContext<TApp>(app).BuildAppAsync(_context.Modules, cancellationToken);
    }
}