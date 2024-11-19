using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Contracts;
using HomeInventory.Domain.Errors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Tests.Systems.Modules;

[UnitTest]
public class AuthenticationModuleTests() : BaseApiModuleTests<AuthenticationModuleTestContext>(static t => new(t))
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
                dataSources.Should().ContainSingle()
                    .Which.Endpoints.OfType<RouteEndpoint>().Should().ContainSingle()
                    .Which.Should().HaveRoutePattern(module.GroupPrefix, RoutePatternFactory.Parse("login"))
                    .And.Subject.Metadata.Should().NotBeEmpty()
                    .And.Subject.GetMetadata<HttpMethodMetadata>().Should().NotBeNull()
                    .And.Subject.HttpMethods.Should().Contain(HttpMethod.Post.Method));
    }

    [Fact]
    public async Task LoginAsync_OnSuccess_ReturnsHttp200()
    {
        await Given
            .HttpContext(out var contextVar)
            .Map<LoginRequest>(out var loginRequestVar).To<AuthenticateQuery>(out var authenticateQueryVar)
            .Map<AuthenticateResult>(out var authenticateResultVar).To<LoginResponse>(out var loginResponseVar)
            .OnQueryReturn(authenticateQueryVar, authenticateResultVar)
            .Sut(out var sutVar)
            .InitializeHostAsync();


        var then = await When
            .InvokedAsync(sutVar, loginRequestVar, contextVar, static (sut, body, context, ct) => sut.LoginAsync(body, context, ct));

        then
            .Result(loginResponseVar, static (actual, expected) =>
                actual.Result.Should().BeOfType<Ok<LoginResponse>>()
                    .Which.Should().HaveValue(expected));
    }

    [Fact]
    public async Task LoginAsync_OnFailure_ReturnsError()
    {
        await Given
            .HttpContext(out var contextVar)
            .Map<LoginRequest>(out var loginRequestVar).To<AuthenticateQuery>(out var authenticateQueryVar)
            .New<InvalidCredentialsError>(out var errorVar)
            .OnQueryReturnError<AuthenticateQuery, AuthenticateResult, InvalidCredentialsError>(authenticateQueryVar, errorVar)
            .Sut(out var sutVar)
            .InitializeHostAsync();

        var then = await When
            .InvokedAsync(sutVar, loginRequestVar, contextVar, (sut, body, context, ct) => sut.LoginAsync(body, context, ct));

        then
            .Result(errorVar, (actual, error) =>
                actual.Result.Should().BeOfType<ProblemHttpResult>()
                    .Which.ProblemDetails.Should().Match(x => x.Title == error.GetType().Name)
                    .And.Match(x => x.Detail == error.Message));
    }
}
