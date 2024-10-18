using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace HomeInventory.Modules.Interfaces;

public interface IModule
{
    IReadOnlyCollection<Type> Dependencies { get; }

    Task AddServicesAsync(IServiceCollection services, IConfiguration configuration, IFeatureManager featureManager, IReadOnlyCollection<IModule> allModules);

    Task BuildAppAsync(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder);
}
