using Asp.Versioning;
using Carter;
using FluentAssertions.Execution;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Contracts;
using HomeInventory.Domain.Errors;
using HomeInventory.Web.Modules;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Systems.Modules;

[UnitTest]
public class UserManagementModuleTests : BaseApiModuleTests
{
    private static readonly Variable<UserIdQuery> _userIdQuery = new(nameof(_userIdQuery));
    private static readonly Variable<UserIdResult> _userIdResult = new(nameof(_userIdResult));
    private static readonly Variable<RegisterResponse> _registerResponse = new(nameof(_registerResponse));
    private static readonly Variable<RegisterRequest> _registerRequest = new(nameof(_registerRequest));
    private static readonly Variable<RegisterCommand> _registerCommand = new(nameof(_registerCommand));

    private static readonly Variable<DuplicateEmailError> _error = new(nameof(_error));

    [Fact]
    public void AddRoutes_ShouldRegister()
    {
        var dataSources = new List<EndpointDataSource>();
        var services = new ServiceCollection()
            .AddOptions(new ApiVersioningOptions())
            .AddSubstitute<IReportApiVersions>()
            .AddSubstitute<IApiVersionParameterSource>()
            .AddSubstitute<IValidatorLocator>();

        var provider = services.BuildServiceProvider();
        var builder = Substitute.For<IEndpointRouteBuilder>();
        builder.ServiceProvider.Returns(provider);
        builder.DataSources.Returns(dataSources);
        var sut = new UserManagementModule();

        sut.AddRoutes(builder);

        using var scope = new AssertionScope();
        var dataSource = dataSources.Should().ContainSingle().Subject;
        var endpoint = dataSource.Endpoints.OfType<RouteEndpoint>().Should().ContainSingle().Subject;
        endpoint.Metadata.Should().NotBeEmpty();
        var method = endpoint.Metadata.GetMetadata<HttpMethodMetadata>()!;
        method.Should().NotBeNull();
        method.HttpMethods.Should().Contain(HttpMethod.Post.Method);
        endpoint.RoutePattern.RawText.Should().Be(RoutePatternFactory.Combine(sut.GroupPrefix, RoutePatternFactory.Parse("register")).RawText);
    }

    [Fact]
    public async Task RegisterAsync_OnSuccess_ReturnsHttp200()
    {
        Given
            .Map(_registerRequest, _registerCommand)
            .Map(_registerRequest, _userIdQuery)
            .Map(_userIdResult, _registerResponse)
            .OnCommandReturnSuccess(_registerCommand)
            .OnQueryReturn(_userIdQuery, _userIdResult);

        var then = await When
            .InvokedAsync(_registerRequest, Given.Context, UserManagementModule.RegisterAsync);

        then
            .Result(_registerResponse, (actual, expected) =>
                actual.Result.Should().BeOfType<Ok<RegisterResponse>>()
                    .Which.Should().HaveValue(expected));
    }

    [Fact]
    public async Task RegisterAsync_OnFailure_ReturnsError()
    {
        Given
            .Map(_registerRequest, _registerCommand)
            .New(_error)
            .OnCommandReturnError(_registerCommand, _error);

        var then = await When
            .InvokedAsync(_registerRequest, Given.Context, UserManagementModule.RegisterAsync);

        then
            .Result(_error, (actual, error) =>
                actual.Result.Should().BeOfType<ProblemHttpResult>()
                    .Which.ProblemDetails.Should().Match(x => x.Title == error.GetType().Name)
                    .And.Match(x => x.Detail == error.Message));
    }
}
