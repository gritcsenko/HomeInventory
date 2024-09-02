using HomeInventory.Application;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HomeInventory.Web.Mapping;

public sealed class WebMappingModule : BaseModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMappingAssemblySource(Assembly.GetExecutingAssembly());
        services.AddAutoMapper((sp, configExpression) =>
        {
            configExpression.AddMaps(sp.GetServices<IMappingAssemblySource>().SelectMany(s => s.GetAssemblies()));
            configExpression.ConstructServicesUsing(sp.GetService);
        }, Type.EmptyTypes);
    }
}
