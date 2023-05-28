using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Contracts;
using HomeInventory.Domain.Errors;
using HomeInventory.Web.Modules;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HomeInventory.Tests.Systems.Controllers;

[UnitTest]
public class AuthenticationModuleTests : BaseApiModuleTests
{
    private static readonly Variable<AuthenticateResult> _authenticateResult = new(nameof(_authenticateResult));
    private static readonly Variable<LoginResponse> _loginResponse = new(nameof(_loginResponse));
    private static readonly Variable<LoginRequest> _loginRequest = new(nameof(_loginRequest));
    private static readonly Variable<AuthenticateQuery> _authenticateQuery = new(nameof(_authenticateQuery));

    private static readonly Variable<InvalidCredentialsError> _error = new(nameof(_error));

    [Fact]
    public async Task LoginAsync_OnSuccess_ReturnsHttp200()
    {
        Given
            .Map(_loginRequest, _authenticateQuery)
            .Map(_authenticateResult, _loginResponse)
            .OnQueryReturn(_authenticateQuery, _authenticateResult);

        var then = await When
            .InvokedAsync(Given.Context, _loginRequest, AuthenticationModule.LoginAsync);

        then
            .Result(_loginResponse, (actual, expected) =>
                actual.Result.Should().BeOfType<Ok<LoginResponse>>()
                    .Which.Should().HaveValue(expected));
    }

    [Fact]
    public async Task LoginAsync_OnFailure_ReturnsError()
    {
        Given
            .Map(_loginRequest, _authenticateQuery)
            .New(_error)
            .OnQueryReturnError<AuthenticateQuery, AuthenticateResult, InvalidCredentialsError>(_authenticateQuery, _error);

        var then = await When
            .InvokedAsync(Given.Context, _loginRequest, AuthenticationModule.LoginAsync);

        then
            .Result(_error, (actual, error) =>
                actual.Result.Should().BeOfType<ProblemHttpResult>()
                    .Which.ProblemDetails.Should().Match(x => x.Title == error.GetType().Name)
                    .And.Match(x => x.Detail == error.Message));
    }
}
