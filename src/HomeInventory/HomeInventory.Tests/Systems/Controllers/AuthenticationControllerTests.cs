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

public class AuthenticationControllerTests
{
    private readonly Fixture _fixture;
    private readonly ISender _mediator;
    private readonly RegisterRequest _registerRequest;
    private readonly LoginRequest _loginRequest;

    public AuthenticationControllerTests()
    {
        _fixture = new Fixture();
        _mediator = Substitute.For<ISender>();

        _registerRequest = _fixture.Create<RegisterRequest>();
        _loginRequest = _fixture.Create<LoginRequest>();
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
            && r.Password == _registerRequest.Password))
            .Returns(_fixture.Create<RegistrationResult>());

        var sut = CreateSut();
        // When
        var result = await sut.RegisterAsync(_registerRequest);
        // Then
        result.Should().BeAssignableTo<IStatusCodeActionResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task LoginAsync_OnSuccess_ReturnsHttp200()
    {
        // Given
        _mediator.Send(Arg.Is<AuthenticateQuery>(r => r.Email == _loginRequest.Email && r.Password == _loginRequest.Password))
            .Returns(_fixture.Create<AuthenticateResult>());

        var sut = CreateSut();
        // When
        var result = await sut.LoginAsync(_loginRequest);
        // Then
        result.Should().BeAssignableTo<IStatusCodeActionResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task RegisterAsync_OnSuccess_ReturnsRegistrationId()
    {
        // Given
        var expectedId = _fixture.Create<Guid>();

        _mediator.Send(Arg.Is<RegisterCommand>(r =>
            r.FirstName == _registerRequest.FirstName
            && r.LastName == _registerRequest.LastName
            && r.Email == _registerRequest.Email
            && r.Password == _registerRequest.Password))
            .Returns(new RegistrationResult(expectedId));

        var sut = CreateSut();
        // When
        var result = await sut.RegisterAsync(_registerRequest);
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
        var expectedId = _fixture.Create<Guid>();
        var expectedToken = _fixture.Create<string>();

        _mediator.Send(Arg.Is<AuthenticateQuery>(r => r.Email == _loginRequest.Email && r.Password == _loginRequest.Password))
            .Returns(new AuthenticateResult(expectedId, expectedToken));

        var sut = CreateSut();
        // When
        var result = await sut.LoginAsync(_loginRequest);
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
            && r.Password == _registerRequest.Password))
            .Returns(error);

        var sut = CreateSut(withHttpContext: true);
        // When
        var result = await sut.RegisterAsync(_registerRequest);
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

        _mediator.Send(Arg.Is<AuthenticateQuery>(r => r.Email == _loginRequest.Email && r.Password == _loginRequest.Password))
            .Returns(error);

        var sut = CreateSut(withHttpContext: true);
        // When
        var result = await sut.LoginAsync(_loginRequest);
        // Then
        var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
        objectResult.StatusCode.Should().NotBe(StatusCodes.Status200OK);
        var details = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        details.Title.Should().Be(error.Code);
        details.Detail.Should().Be(error.Description);
    }
}