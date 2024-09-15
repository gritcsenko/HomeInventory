using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Infrastructure.Framework;

namespace HomeInventory.Tests.Modules;

public class InfrastructureUserManagementModuleTests() : BaseModuleTest<InfrastructureUserManagementModule>(() => new())
{
    protected override void EnsureRegistered(IServiceCollection services)
    {
        services.Should().ContainSingleScoped<IPasswordHasher>()
            .And.ContainSingleSingleton<IJsonDerivedTypeInfo>();
    }
}
