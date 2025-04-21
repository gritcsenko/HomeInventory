using HomeInventory.Application.Framework;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Events;
using HomeInventory.Domain.Persistence;
using HomeInventory.Infrastructure.Framework;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.Services;
using HomeInventory.Infrastructure.UserManagement.Models.Configurations;

namespace Microsoft.Extensions.DependencyInjection;

public static class InfrastructureUserManagementServiceCollectionExtensions
{
    public static IServiceCollection AddUserManagementInfrastructure(this IServiceCollection services)
    {
        services.AddRepository<User, IUserRepository, UserRepository>();
        services.AddMappingAssemblySource(HomeInventory.Infrastructure.UserManagement.AssemblyReference.Assembly);

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IJsonDerivedTypeInfo>(static _ => new DomainEventJsonTypeInfo(typeof(UserCreatedDomainEvent)));

        services.AddScoped<IDatabaseConfigurationApplier, UserModelDatabaseConfigurationApplier>();

        return services;
    }
}
