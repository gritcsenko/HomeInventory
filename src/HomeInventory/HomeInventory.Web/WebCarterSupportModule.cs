using Carter;
using HomeInventory.Modules.Interfaces;
using HomeInventory.Web.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web;

public sealed class WebCarterSupportModule : BaseModule, IAttachableModule
{
    private IReadOnlyCollection<IModule> _modules = [];

    public void OnAttached(IReadOnlyCollection<IModule> modules)
    {
        _modules = modules;
    }

    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCarter(assemblyCatalog: null, configurator => {
            foreach (var module in _modules.OfType<IModuleWithCarter>())
            {
                module.Configure(configurator);
            }
        });
    }

    public override void BuildApp(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapCarter();
    }
}
