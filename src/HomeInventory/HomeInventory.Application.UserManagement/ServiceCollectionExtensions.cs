using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application.UserManagement;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserManagementApplication(this IServiceCollection services)
    {
        services.AddMappingAssemblySource(AssemblyReference.Assembly);
        return services;
    }
}
