using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HomeInventory.Infrastructure;

public sealed class InfrastructureMappingModule : BaseModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AmountObjectConverter>();
        services.AddMappingAssemblySource(Assembly.GetExecutingAssembly());
    }
}
