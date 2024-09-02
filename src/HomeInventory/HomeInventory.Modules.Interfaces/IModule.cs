using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Modules.Interfaces;

public interface IModule
{
    void AddServices(IServiceCollection services, IConfiguration configuration);

    void BuildApp(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder);
}

public interface IAttachableModule : IModule
{
    void OnAttached(IReadOnlyCollection<IModule> modules);
}

public abstract class BaseModule : IModule
{
    public virtual void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public virtual void BuildApp(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder)
    {
    }
}
