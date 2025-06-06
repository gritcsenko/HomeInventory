using HomeInventory.Tests.Systems.Modules;
using HomeInventory.Web.Modules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace HomeInventory.Tests.Presentation.Web;

[UnitTest]
public sealed class PermissionModuleTests() : BaseApiModuleTests<PermissionModuleTestsGivenContext>(t => new(t))
{
    [Fact]
    public async Task AddRoutes_ShouldRegister()
    {
        await Given
            .DataSources(out var dataSourcesVar)
            .RouteBuilder(out var routeBuilderVar, dataSourcesVar)
            .Sut(out var sutVar)
            .InitializeHostAsync();

        var then = When
            .Invoked(sutVar, routeBuilderVar, static (sut, routeBuilder) => sut.AddRoutes(routeBuilder));

        then
            .Ensure(sutVar, dataSourcesVar, static (module, dataSources) =>
            {
                var metadata = dataSources.Should().ContainSingle()
                    .Which.Endpoints.OfType<RouteEndpoint>().Should().ContainSingle()
                    .Which.Should().HaveRoutePattern(module.GroupPrefix, RoutePatternFactory.Parse(""))
                    .And.Subject.Metadata.Should().NotBeEmpty();

                metadata
                    .And.Subject.GetMetadata<AuthorizeAttribute>().Should().NotBeNull()
                    .And.Subject.Policy.Should().Be("Dynamic");

                metadata
                    .And.Subject.GetMetadata<HttpMethodMetadata>().Should().NotBeNull()
                    .And.Subject.HttpMethods.Should().Contain(HttpMethod.Get.Method);
            });
    }

    [Fact]
    public async Task GetPermissionsAsync_ShouldReturnPermissions()
    {
        Given
            .Permissions(out var permissionsVar)
            .PermissionList(out var permissionListVar, permissionsVar)
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, permissionListVar, static (_, permissionList, ct) => PermissionModule.GetPermissionsAsync(permissionList, ct));

        then.Result(permissionsVar, static (ok, permissions) => ok.Value.Should().BeEquivalentTo(permissions.Select(x => x.ToString())));
    }
}
