using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HomeInventory.Web.UserManagement;

public sealed class WebUerManagementMappingModule : BaseModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMappingAssemblySource(Assembly.GetExecutingAssembly());
    }
}
