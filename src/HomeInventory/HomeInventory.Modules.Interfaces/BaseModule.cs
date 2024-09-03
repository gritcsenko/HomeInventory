using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Modules.Interfaces;

public abstract class BaseModule : IModule
{
    public virtual void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public virtual void BuildApp(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder)
    {
    }
}
