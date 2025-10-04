using System.Diagnostics.CodeAnalysis;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Infrastructure.Framework.Models.Configuration;
using HomeInventory.Infrastructure.UserManagement;

namespace HomeInventory.Tests.Modules;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class InfrastructureUserManagementDatabaseModuleTests() : BaseModuleTest<InfrastructureUserManagementDatabaseModule>(static () => new())
{
    protected override void EnsureRegistered(IServiceCollection services) =>
        services.Should().ContainSingleScoped<IUserRepository>()
            .And.ContainSingleScoped<IDatabaseConfigurationApplier>();
}
