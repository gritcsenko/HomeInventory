using System.Diagnostics.CodeAnalysis;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Infrastructure.Framework;
using HomeInventory.Infrastructure.UserManagement;

namespace HomeInventory.Tests.Modules;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class InfrastructureUserManagementModuleTests() : BaseModuleTest<InfrastructureUserManagementModule>(static () => new())
{
    protected override void EnsureRegistered(IServiceCollection services) =>
        services.Should().ContainSingleSingleton<IPasswordHasher>()
            .And.ContainSingleSingleton<IJsonDerivedTypeInfo>();
}
