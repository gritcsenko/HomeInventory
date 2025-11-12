using System.Diagnostics.CodeAnalysis;
using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
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
    [SuppressMessage("Non-substitutable member", "NS1004:Argument matcher used with a non-virtual member of a class.")]
    public async Task LoginAsync_OnSuccess_ReturnsHttp200()
    {
        await Given
            .HttpContext(out var contextVar)
            .New<LoginRequest>(out var loginRequestVar)
            .New<AuthenticateResult>(out var authenticateResultVar)
            // .Map<LoginRequest>(out var loginRequestVar).To<AuthenticateQuery>(out var authenticateQueryVar)
            // .Map<AuthenticateResult>(out var authenticateResultVar).To<LoginResponse>(out var loginResponseVar)
            .SubstituteFor(out IVariable<IUserService> userServiceVar, loginRequestVar, authenticateResultVar, (s, l, r) =>
                s.AuthenticateAsync(Arg.Is<AuthenticateQuery>(q => q.Email.Value == l.Email && q.Password == l.Password), Cancellation.Token)
                    .Returns(QueryResult.From(r)))
            .Sut(out var sutVar)
            .InitializeHostAsync();

        var then = await When
            .InvokedAsync(sutVar, loginRequestVar, userServiceVar, contextVar, static (sut, body, userService, context, ct) => sut.LoginAsync(body, userService, null!, null!, context, ct));

        then
            .Result(authenticateResultVar, static (actual, expected) =>
                actual.Result.Should().BeOfType<Ok<LoginResponse>>()
                    .Which.Value!.Id.Should().Be(expected.Id.ToString()));
    }

    [Fact]
    [SuppressMessage("Non-substitutable member", "NS1004:Argument matcher used with a non-virtual member of a class.")]
    public async Task LoginAsync_OnFailure_ReturnsError()
    {
        await Given
            .HttpContext(out var contextVar)
            .New<LoginRequest>(out var loginRequestVar)
            .New<InvalidCredentialsError>(out var errorVar)
            .SubstituteFor(out IVariable<IUserService> userServiceVar, loginRequestVar, errorVar, (s, l, e) =>
                s.AuthenticateAsync(Arg.Is<AuthenticateQuery>(q => q.Email.Value == l.Email && q.Password == l.Password), Cancellation.Token)
                    .Returns(QueryResult.From<AuthenticateResult>(e)))
            .Sut(out var sutVar)
            .InitializeHostAsync();

        var then = await When
            .InvokedAsync(sutVar, loginRequestVar, userServiceVar, contextVar, (sut, body, userService, context, ct) => sut.LoginAsync(body, userService, null!, null!, context, ct));

        then
            .Result(errorVar, (actual, error) =>
                actual.Result.Should().BeOfType<ProblemHttpResult>()
                    .Which.ProblemDetails.Should().Match(x => x.Title == error.GetType().Name)
                    .And.Match(x => x.Detail == error.Message));
    }
}
