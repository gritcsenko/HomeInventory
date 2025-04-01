using HomeInventory.Application.UserManagement.Interfaces.Commands;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Contracts.UserManagement;
using HomeInventory.Domain.UserManagement.Errors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace HomeInventory.Tests.Systems.Modules;

[UnitTest]
public class UserManagementModuleTests() : BaseApiModuleTests<UserManagementModuleTestContext>(static t => new(t))
{
    [Fact]
    public async Task AddRoutes_ShouldRegister()
    {
        await Given
            .DataSources(out var dataSourcesVar)
            .RouteBuilder(out var routeBuilderVar, dataSourcesVar)
            .InitializeHostAsync();
        Given.Sut(out var sutVar);

        var then = When
            .Invoked(sutVar, routeBuilderVar, static (sut, routeBuilder) => sut.AddRoutes(routeBuilder));

        then
            .Ensure(sutVar, dataSourcesVar, static (module, dataSources) =>
                dataSources.Should().ContainSingle()
                    .Which.Endpoints.OfType<RouteEndpoint>().Should().ContainSingle()
                    .Which.Should().HaveRoutePattern(module.GroupPrefix, RoutePatternFactory.Parse("register"))
                    .And.Subject.Metadata.Should().NotBeEmpty()
                    .And.Subject.GetMetadata<HttpMethodMetadata>().Should().NotBeNull()
                    .And.Subject.HttpMethods.Should().Contain(HttpMethod.Post.Method));
    }

    [Fact]
    public async Task RegisterAsync_OnSuccess_ReturnsHttp200()
    {
        await Given
            .Map<RegisterRequest>(out var registerRequestVar).To<RegisterCommand>(out var registerCommandVar)
            .Map(registerRequestVar).To<UserIdQuery>(out var userIdQueryVar)
            .Map<UserIdResult>(out var userIdResultVar).To<RegisterResponse>(out var registerResponseVar)
            .OnCommandReturnSuccess(registerCommandVar)
            .OnQueryReturn(userIdQueryVar, userIdResultVar)
            .InitializeHostAsync();
        Given
            .HttpContext(out var contextVar)
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, registerRequestVar, contextVar, static (sut, body, context, ct) => sut.RegisterAsync(body, null!, null!, context, ct));

        then
            .Result(registerResponseVar, static (actual, expected) =>
                actual.Result.Should().BeOfType<Ok<RegisterResponse>>()
                    .Which.Should().HaveValue(expected));
    }

    [Fact]
    public async Task RegisterAsync_OnFailure_ReturnsError()
    {
        await Given
            .Map<RegisterRequest>(out var registerRequestVar).To<RegisterCommand>(out var registerCommandVar)
            .New<DuplicateEmailError>(out var errorVar)
            .OnCommandReturnError(registerCommandVar, errorVar)
            .InitializeHostAsync();
        Given
            .HttpContext(out var contextVar)
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, registerRequestVar, contextVar, (sut, body, context, ct) => sut.RegisterAsync(body, null!, null!, context, ct));

        then
            .Result(errorVar, (actual, error) =>
                actual.Result.Should().BeOfType<ProblemHttpResult>()
                    .Which.ProblemDetails.Should().Match(x => x.Title == error.GetType().Name)
                    .And.Match(x => x.Detail == error.Message));
    }
}
