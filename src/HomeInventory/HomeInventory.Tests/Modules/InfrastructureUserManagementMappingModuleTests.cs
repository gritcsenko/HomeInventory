using HomeInventory.Application.Framework;
using HomeInventory.Infrastructure.UserManagement;

namespace HomeInventory.Tests.Modules;

public class InfrastructureUserManagementMappingModuleTests() : BaseModuleTest<InfrastructureUserManagementMappingModule>(static () => new())
{
    protected override void EnsureRegistered(IServiceCollection services) => services.Should().ContainSingleSingleton<IMappingAssemblySource>();
}
