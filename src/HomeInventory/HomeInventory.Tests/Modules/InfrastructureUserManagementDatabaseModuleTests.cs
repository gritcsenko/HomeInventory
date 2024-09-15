using HomeInventory.Domain.Persistence;
using HomeInventory.Infrastructure.Persistence;

namespace HomeInventory.Tests.Modules;

public class InfrastructureUserManagementDatabaseModuleTests() : BaseModuleTest<InfrastructureUserManagementDatabaseModule>(() => new())
{
    protected override void EnsureRegistered(IServiceCollection services)
    {
        services.Should().ContainSingleScoped<IUserRepository>()
            .And.ContainSingleScoped<IDatabaseConfigurationApplier>();
    }
}
