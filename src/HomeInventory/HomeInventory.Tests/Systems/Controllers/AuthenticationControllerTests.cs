using AutoFixture;
using FluentAssertions;
using HomeInventory.Api.Controllers;
using HomeInventory.Application.Services.Authentication;
using HomeInventory.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NSubstitute;

namespace HomeInventory.Tests.Systems.Controllers;

public class AuthenticationControllerTests
{
    private readonly Fixture _fixture;
    private readonly IAuthenticationService _service;

    public AuthenticationControllerTests()
    {
        _fixture = new Fixture();
        _service = Substitute.For<IAuthenticationService>();
    }

    [Fact]
    public async Task RegisterAsync_OnSuccess_ReturnsHttp200()
    {
        // Given
        var body = _fixture.Create<RegisterRequest>();
        var id = _fixture.Create<Guid>();

        _service.RegisterAsync(body.FirstName, body.LastName, body.Email, body.Password)
            .Returns(new RegistrationResult(id));

        var sut = new AuthenticationController(_service);
        // When
        var result = await sut.RegisterAsync(body);
        // Then
        result.Should().BeAssignableTo<IStatusCodeActionResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task LoginAsync_OnSuccess_ReturnsHttp200()
    {
        // Given
        var body = _fixture.Create<LoginRequest>();
        var id = _fixture.Create<Guid>();
        var token = _fixture.Create<string>();

        _service.AuthenticateAsync(body.Email, body.Password)
            .Returns(new AuthenticateResult(id, token));

        var sut = new AuthenticationController(_service);
        // When
        var result = await sut.LoginAsync(body);
        // Then
        result.Should().BeAssignableTo<IStatusCodeActionResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task RegisterAsync_OnSuccess_ReturnsRegistrationId()
    {
        // Given
        var body = _fixture.Create<RegisterRequest>();
        var expectedId = _fixture.Create<Guid>();

        _service.RegisterAsync(body.FirstName, body.LastName, body.Email, body.Password)
            .Returns(new RegistrationResult(expectedId));

        var sut = new AuthenticationController(_service);
        // When
        var result = await sut.RegisterAsync(body);
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
        var body = _fixture.Create<LoginRequest>();
        var expectedId = _fixture.Create<Guid>();
        var expectedToken = _fixture.Create<string>();

        _service.AuthenticateAsync(body.Email, body.Password)
            .Returns(new AuthenticateResult(expectedId, expectedToken));

        var sut = new AuthenticationController(_service);
        // When
        var result = await sut.LoginAsync(body);
        // Then
        result.Should().BeAssignableTo<ObjectResult>()
            .Which.Value.Should().BeOfType<LoginResponse>()
            .Subject.Deconstruct(out var actualId, out var actualToken);
        actualId.Should().Be(expectedId);
        actualToken.Should().Be(expectedToken);
    }
}