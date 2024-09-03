using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public sealed class InfrastructureUserManagementMappingModule : BaseModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMappingAssemblySource(Assembly.GetExecutingAssembly());
    }
}
