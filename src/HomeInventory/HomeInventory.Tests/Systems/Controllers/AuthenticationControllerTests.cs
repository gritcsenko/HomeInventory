using AutoFixture;
using FluentAssertions;
using HomeInventory.Api.Controllers;
using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Contracts;
using HomeInventory.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NSubstitute;

namespace HomeInventory.Tests.Systems.Controllers;

public class AuthenticationControllerTests : BaseTest
{
    private readonly ISender _mediator = Substitute.For<ISender>();
    private readonly RegisterRequest _registerRequest;
    private readonly LoginRequest _loginRequest;

    public AuthenticationControllerTests()
    {
        _registerRequest = Fixture.Create<RegisterRequest>();
        _loginRequest = Fixture.Create<LoginRequest>();
    }

    private AuthenticationController CreateSut(bool withHttpContext = false)
    {
        var sut = new AuthenticationController(_mediator);
        if (withHttpContext)
        {
            sut.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() };
        }

        return sut;
    }

    [Fact]
    public async Task RegisterAsync_OnSuccess_ReturnsHttp200()
    {
        // Given
        _mediator.Send(Arg.Is<RegisterCommand>(r =>
            r.FirstName == _registerRequest.FirstName
            && r.LastName == _registerRequest.LastName
            && r.Email == _registerRequest.Email
            && r.Password == _registerRequest.Password), CancellationToken)
            .Returns(Fixture.Create<RegistrationResult>());

        var sut = CreateSut();
        // When
        var result = await sut.RegisterAsync(_registerRequest, CancellationToken);
        // Then
        result.Should().BeAssignableTo<IStatusCodeActionResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task LoginAsync_OnSuccess_ReturnsHttp200()
    {
        // Given
        _mediator.Send(Arg.Is<AuthenticateQuery>(r => r.Email == _loginRequest.Email && r.Password == _loginRequest.Password), CancellationToken)
            .Returns(Fixture.Create<AuthenticateResult>());

        var sut = CreateSut();
        // When
        var result = await sut.LoginAsync(_loginRequest, CancellationToken);
        // Then
        result.Should().BeAssignableTo<IStatusCodeActionResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task RegisterAsync_OnSuccess_ReturnsRegistrationId()
    {
        // Given
        var expectedId = Fixture.Create<Guid>();

        _mediator.Send(Arg.Is<RegisterCommand>(r =>
            r.FirstName == _registerRequest.FirstName
            && r.LastName == _registerRequest.LastName
            && r.Email == _registerRequest.Email
            && r.Password == _registerRequest.Password), CancellationToken)
            .Returns(new RegistrationResult(expectedId));

        var sut = CreateSut();
        // When
        var result = await sut.RegisterAsync(_registerRequest, CancellationToken);
        // Then
        result.Should().BeAssignableTo<ObjectResult>()
            .Which.Value.Should().BeOfType<RegisterResponse>()
            .Subject.Deconstruct(out var actualId);
        actualId.Should().Be(expectedId);
    }

    [Fact]
    public async Task LoginAsync_OnSuccess_ReturnsRegistrationIdAndToken()
    {
        // Given
        var expectedId = Fixture.Create<Guid>();
        var expectedToken = Fixture.Create<string>();

        _mediator.Send(Arg.Is<AuthenticateQuery>(r => r.Email == _loginRequest.Email && r.Password == _loginRequest.Password), CancellationToken)
            .Returns(new AuthenticateResult(expectedId, expectedToken));

        var sut = CreateSut();
        // When
        var result = await sut.LoginAsync(_loginRequest, CancellationToken);
        // Then
        result.Should().BeAssignableTo<ObjectResult>()
            .Which.Value.Should().BeOfType<LoginResponse>()
            .Subject.Deconstruct(out var actualId, out var actualToken);
        actualId.Should().Be(expectedId);
        actualToken.Should().Be(expectedToken);
    }

    [Fact]
    public async Task RegisterAsync_OnFailure_ReturnsError()
    {
        // Given
        var error = Errors.User.DuplicateEmail;

        _mediator.Send(Arg.Is<RegisterCommand>(r =>
            r.FirstName == _registerRequest.FirstName
            && r.LastName == _registerRequest.LastName
            && r.Email == _registerRequest.Email
            && r.Password == _registerRequest.Password), CancellationToken)
            .Returns(error);

        var sut = CreateSut(withHttpContext: true);
        // When
        var result = await sut.RegisterAsync(_registerRequest, CancellationToken);
        // Then
        var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
        objectResult.StatusCode.Should().NotBe(StatusCodes.Status200OK);
        var details = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        details.Title.Should().Be(error.Code);
        details.Detail.Should().Be(error.Description);
    }

    [Fact]
    public async Task LoginAsync_OnFailure_ReturnsError()
    {
        // Given
        var error = Errors.Authentication.InvalidCredentials;

        _mediator.Send(Arg.Is<AuthenticateQuery>(r => r.Email == _loginRequest.Email && r.Password == _loginRequest.Password), CancellationToken)
            .Returns(error);

        var sut = CreateSut(withHttpContext: true);
        // When
        var result = await sut.LoginAsync(_loginRequest, CancellationToken);
        // Then
        var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
        objectResult.StatusCode.Should().NotBe(StatusCodes.Status200OK);
        var details = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        details.Title.Should().Be(error.Code);
        details.Detail.Should().Be(error.Description);
    }
}