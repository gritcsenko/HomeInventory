﻿using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Contracts;
using HomeInventory.Domain.Errors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace HomeInventory.Tests.Systems.Modules;

[UnitTest]
public class UserManagementModuleTests() : BaseApiModuleTests<UserManagementModuleTestContext>(t => new(t))
{
    [Fact]
    public void AddRoutes_ShouldRegister()
    {
        Given
            .DataSources(out var dataSources)
            .RouteBuilder(out var routeBuilder, dataSources)
            .Sut(out var sut);

        var then = When
            .Invoked(sut, routeBuilder, (sut, routeBuilder) => sut.AddRoutes(routeBuilder));

        then
            .Ensure(sut, dataSources, (module, dataSources) =>
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
        Given
            .Map<RegisterRequest>(out var registerRequest).To<RegisterUserRequestMessage>(out var registerCommand)
            .Map(registerRequest).To<UserIdQueryMessage>(out var userIdQuery)
            .Map<UserIdResult>(out var userIdResult).To<RegisterResponse>(out var registerResponse)
            .OnCommandReturnSuccess(registerCommand)
            .OnQueryReturn(userIdQuery, userIdResult)
            .HttpContext(out var httpContext)
            .Sut(out var sut);

        var then = await When
            .InvokedAsync(sut, registerRequest, httpContext, (sut, body, ctx, ct) => sut.RegisterAsync(body, null!, ctx, ct));

        then
            .Result(registerResponse, (actual, expected) =>
                actual.Result.Should().BeOfType<Ok<RegisterResponse>>()
                    .Which.Should().HaveValue(expected));
    }

    [Fact]
    public async Task RegisterAsync_OnFailure_ReturnsError()
    {
        Given
            .Map<RegisterRequest>(out var registerRequest).To<RegisterUserRequestMessage>(out var registerCommand)
            .New<DuplicateEmailError>(out var error)
            .OnCommandReturnError(registerCommand, error)
            .HttpContext(out var httpContext)
            .Sut(out var sut);

        var then = await When
            .InvokedAsync(sut, registerRequest, httpContext, (sut, body, ctx, ct) => sut.RegisterAsync(body, null!, ctx, ct));

        then
            .Result(error, (actual, error) =>
                actual.Result.Should().BeOfType<ProblemHttpResult>()
                    .Which.ProblemDetails.Should().Match(x => x.Title == error.GetType().Name)
                    .And.Match(x => x.Detail == error.Message));
    }
}
