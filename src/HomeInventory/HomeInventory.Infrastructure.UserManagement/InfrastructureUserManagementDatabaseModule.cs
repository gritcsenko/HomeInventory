using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Infrastructure.Framework;
using HomeInventory.Infrastructure.Framework.Models.Configuration;
using HomeInventory.Infrastructure.UserManagement.Models.Configurations;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Infrastructure.UserManagement;

public sealed class InfrastructureUserManagementDatabaseModule : BaseModule
{
    public override async Task AddServicesAsync(ModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services
            .AddRepository<User, IUserRepository, UserRepository>()
            .AddScoped<IDatabaseConfigurationApplier, UserModelDatabaseConfigurationApplier>();
    }
}
