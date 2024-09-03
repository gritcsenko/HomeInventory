using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Persistence;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public sealed class InfrastructureUserManagementDatabaseModule : BaseModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepository<User, IUserRepository, UserRepository>();
        services.AddScoped<IDatabaseConfigurationApplier, UserModelDatabaseConfigurationApplier>();
    }
}
