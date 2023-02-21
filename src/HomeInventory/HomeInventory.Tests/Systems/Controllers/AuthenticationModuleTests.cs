using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Contracts;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Helpers;
using HomeInventory.Web.Modules;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace HomeInventory.Tests.Systems.Controllers;

[Trait("Category", "Unit")]
public class AuthenticationModuleTests : BaseTest
{
    private readonly ISender _mediator = Substitute.For<ISender>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IValidator<RegisterRequest> _registerValidator = Substitute.For<IValidator<RegisterRequest>>();
    private readonly IValidator<LoginRequest> _loginValidator = Substitute.For<IValidator<LoginRequest>>();

    private readonly RegisterRequest _registerRequest;
    private readonly RegisterCommand _registerCommand;
    private readonly LoginRequest _loginRequest;
    private readonly AuthenticateQuery _authenticateQuery;
    private readonly HttpContext _context;

    public AuthenticationModuleTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();

        _registerRequest = Fixture.Create<RegisterRequest>();
        _registerCommand = Fixture.Create<RegisterCommand>();
        _loginRequest = Fixture.Create<LoginRequest>();
        _authenticateQuery = Fixture.Create<AuthenticateQuery>();

        _mapper.Map<RegisterCommand>(_registerRequest).Returns(_registerCommand);
        _mapper.Map<AuthenticateQuery>(_loginRequest).Returns(_authenticateQuery);

        _registerValidator.ValidateAsync(_registerRequest, CancellationToken).Returns(new ValidationResult());
        _loginValidator.ValidateAsync(_loginRequest, CancellationToken).Returns(new ValidationResult());

        var collection = new ServiceCollection();
        collection.AddSingleton(_mediator);
        collection.AddSingleton(_mapper);
        collection.AddSingleton(_registerValidator);
        collection.AddSingleton(_loginValidator);

        _context = new DefaultHttpContext
        {
            RequestServices = collection.BuildServiceProvider()
        };
    }

    [Fact]
    public async Task RegisterAsync_OnSuccess_ReturnsHttp200()
    {
        // Given
        var registrationResult = Fixture.Create<RegistrationResult>();
        var expectedResultValue = Fixture.Create<RegisterResponse>();
        _mediator.Send(_registerCommand, CancellationToken).Returns(Result.Ok(registrationResult));
        _mapper.Map<RegisterResponse>(registrationResult).Returns(expectedResultValue);
        // When
        var result = await AuthenticationModule.RegisterAsync(_context, _registerRequest, CancellationToken);
        // Then
        result.Should().BeOfType<Ok<RegisterResponse>>()
            .Which.Should().HaveValue(expectedResultValue);
    }

    [Fact]
    public async Task LoginAsync_OnSuccess_ReturnsHttp200()
    {
        // Given
        var authenticationResult = Fixture.Create<AuthenticateResult>();
        var expectedResultValue = Fixture.Create<LoginResponse>();
        _mediator.Send(_authenticateQuery, CancellationToken).Returns(Result.Ok(authenticationResult));
        _mapper.Map<LoginResponse>(authenticationResult).Returns(expectedResultValue);
        // When
        var result = await AuthenticationModule.LoginAsync(_context, _loginRequest, CancellationToken);
        // Then
        result.Should().BeOfType<Ok<LoginResponse>>()
            .Which.Should().HaveValue(expectedResultValue);
    }

    [Fact]
    public async Task RegisterAsync_OnSuccess_ReturnsRegistrationId()
    {
        // Given
        var registrationResult = Fixture.Create<RegistrationResult>();
        var expectedResultValue = Fixture.Create<RegisterResponse>();
        _mediator.Send(_registerCommand, CancellationToken).Returns(Result.Ok(registrationResult));
        _mapper.Map<RegisterResponse>(registrationResult).Returns(expectedResultValue);
        // When
        var result = await AuthenticationModule.RegisterAsync(_context, _registerRequest, CancellationToken);
        // Then
        result.Should().BeOfType<Ok<RegisterResponse>>()
            .Which.Should().HaveValue(expectedResultValue);
    }

    [Fact]
    public async Task LoginAsync_OnSuccess_ReturnsRegistrationIdAndToken()
    {
        // Given
        var authenticationResult = Fixture.Create<AuthenticateResult>();
        var expectedResultValue = Fixture.Create<LoginResponse>();
        _mapper.Map<LoginResponse>(authenticationResult).Returns(expectedResultValue);
        _mediator.Send(_authenticateQuery, CancellationToken).Returns(Result.Ok(authenticationResult));
        // When
        var result = await AuthenticationModule.LoginAsync(_context, _loginRequest, CancellationToken);
        // Then
        result.Should().BeOfType<Ok<LoginResponse>>()
            .Which.Should().HaveValue(expectedResultValue);
    }

    [Fact]
    public async Task RegisterAsync_OnFailure_ReturnsError()
    {
        // Given
        var error = new DuplicateEmailError();
        _mediator.Send(_registerCommand, CancellationToken).Returns(Result.Fail<RegistrationResult>(error));
        // When
        var result = await AuthenticationModule.RegisterAsync(_context, _registerRequest, CancellationToken);
        // Then
        result.Should().BeOfType<ProblemHttpResult>()
            .Which.ProblemDetails.Should().Match(x => x.Title == error.GetType().Name)
            .And.Match(x => x.Detail == error.Message);
    }

    [Fact]
    public async Task LoginAsync_OnFailure_ReturnsError()
    {
        // Given
        var error = new InvalidCredentialsError();
        _mediator.Send(_authenticateQuery, CancellationToken).Returns(Result.Fail<AuthenticateResult>(error));
        // When
        var result = await AuthenticationModule.LoginAsync(_context, _loginRequest, CancellationToken);
        // Then
        result.Should().BeOfType<ProblemHttpResult>()
            .Which.ProblemDetails.Should().Match(x => x.Title == error.GetType().Name)
            .And.Match(x => x.Detail == error.Message);
    }
}
