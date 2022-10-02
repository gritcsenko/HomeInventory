using AutoFixture;
using FluentAssertions;
using FluentResults;
using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Contracts;
using HomeInventory.Domain.Errors;
using HomeInventory.Tests.Customizations;
using HomeInventory.Tests.Helpers;
using HomeInventory.Web.Controllers;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace HomeInventory.Tests.Systems.Controllers;

[Trait("Category", "Unit")]
public class AuthenticationControllerTests : BaseTest
{
    private readonly ISender _mediator = Substitute.For<ISender>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly RegisterRequest _registerRequest;
    private readonly RegisterCommand _registerCommand;
    private readonly LoginRequest _loginRequest;
    private readonly AuthenticateQuery _authenticateQuery;

    public AuthenticationControllerTests()
    {
        Fixture.Customize(new UserIdCustomization());

        _registerRequest = Fixture.Create<RegisterRequest>();
        _registerCommand = Fixture.Create<RegisterCommand>();
        _loginRequest = Fixture.Create<LoginRequest>();
        _authenticateQuery = Fixture.Create<AuthenticateQuery>();

        _mapper.Map<RegisterCommand>(_registerRequest).Returns(_registerCommand);
        _mapper.Map<AuthenticateQuery>(_loginRequest).Returns(_authenticateQuery);
    }

    private AuthenticationController CreateSut() => new(_mediator, _mapper);

    [Fact]
    public async Task RegisterAsync_OnSuccess_ReturnsHttp200()
    {
        // Given
        var registrationResult = Fixture.Create<RegistrationResult>();
        var expectedResultValue = Fixture.Create<RegisterResponse>();
        _mediator.Send(_registerCommand, CancellationToken).Returns(registrationResult);
        _mapper.Map<RegisterResponse>(registrationResult).Returns(expectedResultValue);
        var sut = CreateSut();
        // When
        var result = await sut.RegisterAsync(_registerRequest, CancellationToken);
        // Then
        result.Should().HaveStatusCode(StatusCodes.Status200OK)
            .And.HaveValue(expectedResultValue);
    }

    [Fact]
    public async Task LoginAsync_OnSuccess_ReturnsHttp200()
    {
        // Given
        var authenticationResult = Fixture.Create<AuthenticateResult>();
        var expectedResultValue = Fixture.Create<LoginResponse>();
        _mediator.Send(_authenticateQuery, CancellationToken).Returns(authenticationResult);
        _mapper.Map<LoginResponse>(authenticationResult).Returns(expectedResultValue);
        var sut = CreateSut();
        // When
        var result = await sut.LoginAsync(_loginRequest, CancellationToken);
        // Then
        result.Should().HaveStatusCode(StatusCodes.Status200OK)
            .And.HaveValue(expectedResultValue);
    }

    [Fact]
    public async Task RegisterAsync_OnSuccess_ReturnsRegistrationId()
    {
        // Given
        var registrationResult = Fixture.Create<RegistrationResult>();
        var expectedResultValue = Fixture.Create<RegisterResponse>();
        _mediator.Send(_registerCommand, CancellationToken).Returns(registrationResult);
        _mapper.Map<RegisterResponse>(registrationResult).Returns(expectedResultValue);
        var sut = CreateSut();
        // When
        var result = await sut.RegisterAsync(_registerRequest, CancellationToken);
        // Then
        result.Should().HaveStatusCode(StatusCodes.Status200OK)
            .And.HaveValue(expectedResultValue);
    }

    [Fact]
    public async Task LoginAsync_OnSuccess_ReturnsRegistrationIdAndToken()
    {
        // Given
        var authenticationResult = Fixture.Create<AuthenticateResult>();
        var expectedResultValue = Fixture.Create<LoginResponse>();
        _mapper.Map<LoginResponse>(authenticationResult).Returns(expectedResultValue);
        _mediator.Send(_authenticateQuery, CancellationToken).Returns(authenticationResult);
        var sut = CreateSut();
        // When
        var result = await sut.LoginAsync(_loginRequest, CancellationToken);
        // Then
        result.Should().HaveStatusCode(StatusCodes.Status200OK)
            .And.HaveValue(expectedResultValue);
    }

    [Fact]
    public async Task RegisterAsync_OnFailure_ReturnsError()
    {
        // Given
        var error = new DuplicateEmailError();
        _mediator.Send(_registerCommand, CancellationToken).Returns(Result.Fail<RegistrationResult>(error));
        var sut = CreateSut().WithHttpContext();
        // When
        var result = await sut.RegisterAsync(_registerRequest, CancellationToken);
        // Then
        var details = result.Should().NotHaveStatusCode(StatusCodes.Status200OK)
            .And.HaveValueAssignableTo<ProblemDetails>()
            .Which.Should().Match(x => x.Title == error.GetType().Name)
            .And.Match(x => x.Detail == error.Message);
    }

    [Fact]
    public async Task LoginAsync_OnFailure_ReturnsError()
    {
        // Given
        var error = new InvalidCredentialsError();
        _mediator.Send(_authenticateQuery, CancellationToken).Returns(Result.Fail<AuthenticateResult>(error));
        var sut = CreateSut().WithHttpContext();
        // When
        var result = await sut.LoginAsync(_loginRequest, CancellationToken);
        // Then
        var details = result.Should().NotHaveStatusCode(StatusCodes.Status200OK)
            .And.HaveValueAssignableTo<ProblemDetails>()
            .Which.Should().Match(x => x.Title == error.GetType().Name)
            .And.Match(x => x.Detail == error.Message);
    }
}