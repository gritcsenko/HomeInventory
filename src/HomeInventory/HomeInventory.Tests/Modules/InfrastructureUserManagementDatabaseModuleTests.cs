using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Infrastructure.Framework.Models.Configuration;
using HomeInventory.Infrastructure.UserManagement;

namespace HomeInventory.Tests.Modules;

public class InfrastructureUserManagementDatabaseModuleTests() : BaseModuleTest<InfrastructureUserManagementDatabaseModule>(() => new())
{
    protected override void EnsureRegistered(IServiceCollection services)
    {
        services.Should().ContainSingleScoped<IUserRepository>()
            .And.ContainSingleScoped<IDatabaseConfigurationApplier>();
    }
}
