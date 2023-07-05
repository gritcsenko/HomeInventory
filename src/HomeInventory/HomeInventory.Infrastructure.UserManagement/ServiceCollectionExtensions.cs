using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Events;
using HomeInventory.Domain.Persistence;
using HomeInventory.Infrastructure.Framework;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.Services;
using HomeInventory.Infrastructure.UserManagement.Models.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Infrastructure.UserManagement;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserManagementInfrastructure(this IServiceCollection services)
    {
        services.AddRepository<User, IUserRepository, UserRepository>();
        services.AddMappingAssemblySource(AssemblyReference.Assembly);

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IDomainEventJsonTypeInfo>(_ => new DomainEventJsonTypeInfo(typeof(UserCreatedDomainEvent)));

        services.AddScoped<IDatabaseConfigurationApplier, UserModelDatabaseConfigurationApplier>();

        return services;
    }
}
