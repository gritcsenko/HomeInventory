using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Modules.Interfaces;

public abstract class BaseModule : IModule
{
    private readonly List<Type> _dependencies = [];

    public IReadOnlyCollection<Type> Dependencies => _dependencies;

    public virtual void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public virtual void BuildApp(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder)
    {
    }

    protected void DependsOn<TModule>()
        where TModule : class, IModule =>
        _dependencies.Add(typeof(TModule));
}
