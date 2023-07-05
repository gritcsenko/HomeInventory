using HomeInventory.Application;
using HomeInventory.Web.UserManagement;

namespace HomeInventory.Tests.DependencyInjection;

[UnitTest]
public class UserManagementWebDependencyInjectionTests : BaseDependencyInjectionTest
{
    [Fact]
    public void ShouldRegister()
    {
        Services.AddUserManagementWeb();
        var provider = CreateProvider();

        Services.Should().ContainSingleSingleton<IMappingAssemblySource>(provider);
    }
}
