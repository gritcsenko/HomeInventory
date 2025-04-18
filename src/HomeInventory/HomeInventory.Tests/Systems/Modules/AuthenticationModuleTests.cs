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
    public void AddRoutes_ShouldRegister()
    {
        Given
            .DataSources(out var dataSources)
            .RouteBuilder(out var routeBuilder, dataSources)
            .Sut(out var sut);

        var then = When
            .Invoked(sut, routeBuilder, static (sut, routeBuilder) => sut.AddRoutes(routeBuilder));

        then
            .Ensure(sut, dataSources, static (module, dataSources) =>
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
        Given
            .HttpContext(out var context)
            .Map<LoginRequest>(out var loginRequest).To<AuthenticateQuery>(out var authenticateQuery)
            .Map<AuthenticateResult>(out var authenticateResult).To<LoginResponse>(out var loginResponse)
            .OnQueryReturn(authenticateQuery, authenticateResult)
            .Sut(out var sut);


        var then = await When
            .InvokedAsync(sut, loginRequest, context, static (sut, body, context, ct) => sut.LoginAsync(body, context, ct));

        then
            .Result(loginResponse, static (actual, expected) =>
                actual.Result.Should().BeOfType<Ok<LoginResponse>>()
                    .Which.Should().HaveValue(expected));
    }

    [Fact]
    public async Task LoginAsync_OnFailure_ReturnsError()
    {
        Given
            .HttpContext(out var context)
            .Map<LoginRequest>(out var loginRequest).To<AuthenticateQuery>(out var authenticateQuery)
            .New<InvalidCredentialsError>(out var error)
            .OnQueryReturnError<AuthenticateQuery, AuthenticateResult, InvalidCredentialsError>(authenticateQuery, error)
            .Sut(out var sut);

        var then = await When
            .InvokedAsync(sut, loginRequest, context, (sut, body, context, ct) => sut.LoginAsync(body, context, ct));

        then
            .Result(error, (actual, error) =>
                actual.Result.Should().BeOfType<ProblemHttpResult>()
                    .Which.ProblemDetails.Should().Match(x => x.Title == error.GetType().Name)
                    .And.Match(x => x.Detail == error.Message));
    }
}
