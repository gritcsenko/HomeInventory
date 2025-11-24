using System.Diagnostics.CodeAnalysis;
using HomeInventory.Web.Authorization.Dynamic;
using Microsoft.AspNetCore.Authorization;

namespace HomeInventory.Tests.Modules;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class DynamicWebAuthorizationModuleTests() : BaseModuleTest<DynamicWebAuthorizationModule>(static () => new())
{
    protected override void EnsureRegistered(IServiceCollection services) =>
        services.Should()
            .ContainSingleSingleton<PermissionList>()
            .And.ContainTransient<IAuthorizationHandler>();
}

