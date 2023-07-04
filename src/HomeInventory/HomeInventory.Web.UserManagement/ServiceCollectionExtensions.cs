using HomeInventory.Application;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.UserManagement;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserManagementWeb(this IServiceCollection services)
    {
        services.AddMappingAssemblySource(AssemblyReference.Assembly);
        return services;
    }
}
