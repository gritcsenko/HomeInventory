using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Contracts;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Web.Modules;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using OneOf.Types;

namespace HomeInventory.Tests.Systems.Controllers;

[UnitTest]
public class AuthenticationModuleTests : BaseTest
{
    private readonly ISender _mediator = Substitute.For<ISender>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IValidator<RegisterRequest> _registerValidator = Substitute.For<IValidator<RegisterRequest>>();
    private readonly IValidator<LoginRequest> _loginValidator = Substitute.For<IValidator<LoginRequest>>();

    private readonly RegisterRequest _registerRequest;
    private readonly RegisterCommand _registerCommand;
    private readonly UserIdQuery _userIdQuery;
    private readonly LoginRequest _loginRequest;
    private readonly AuthenticateQuery _authenticateQuery;
    private readonly HttpContext _context;

    public AuthenticationModuleTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();

        _registerRequest = Fixture.Create<RegisterRequest>();
        _registerCommand = Fixture.Create<RegisterCommand>();
        _userIdQuery = Fixture.Create<UserIdQuery>();
        _loginRequest = Fixture.Create<LoginRequest>();
        _authenticateQuery = Fixture.Create<AuthenticateQuery>();

        _mapper.Map<RegisterCommand>(_registerRequest).Returns(_registerCommand);
        _mapper.Map<UserIdQuery>(_registerRequest).Returns(_userIdQuery);
        _mapper.Map<AuthenticateQuery>(_loginRequest).Returns(_authenticateQuery);

        _registerValidator.ValidateAsync(_registerRequest, Cancellation.Token).Returns(new ValidationResult());
        _loginValidator.ValidateAsync(_loginRequest, Cancellation.Token).Returns(new ValidationResult());

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
        var userIdResult = Fixture.Create<UserIdResult>();
        var expectedResultValue = Fixture.Create<RegisterResponse>();
        _mediator.Send(_registerCommand, Cancellation.Token).Returns(new Success());
        _mediator.Send(_userIdQuery, Cancellation.Token).Returns(userIdResult);
        _mapper.Map<RegisterResponse>(userIdResult).Returns(expectedResultValue);
        // When
        var result = await AuthenticationModule.RegisterAsync(_context, _registerRequest, Cancellation.Token);
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
        _mediator.Send(_authenticateQuery, Cancellation.Token).Returns(authenticationResult);
        _mapper.Map<LoginResponse>(authenticationResult).Returns(expectedResultValue);
        // When
        var result = await AuthenticationModule.LoginAsync(_context, _loginRequest, Cancellation.Token);
        // Then
        result.Should().BeOfType<Ok<LoginResponse>>()
            .Which.Should().HaveValue(expectedResultValue);
    }

    [Fact]
    public async Task RegisterAsync_OnSuccess_ReturnsRegistrationId()
    {
        // Given
        var userIdResult = Fixture.Create<UserIdResult>();
        var expectedResultValue = Fixture.Create<RegisterResponse>();
        _mediator.Send(_registerCommand, Cancellation.Token).Returns(new Success());
        _mediator.Send(_userIdQuery, Cancellation.Token).Returns(userIdResult);
        _mapper.Map<RegisterResponse>(userIdResult).Returns(expectedResultValue);
        // When
        var result = await AuthenticationModule.RegisterAsync(_context, _registerRequest, Cancellation.Token);
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
        _mediator.Send(_authenticateQuery, Cancellation.Token).Returns(authenticationResult);
        // When
        var result = await AuthenticationModule.LoginAsync(_context, _loginRequest, Cancellation.Token);
        // Then
        result.Should().BeOfType<Ok<LoginResponse>>()
            .Which.Should().HaveValue(expectedResultValue);
    }

    [Fact]
    public async Task RegisterAsync_OnFailure_ReturnsError()
    {
        // Given
        var error = Fixture.Create<DuplicateEmailError>();
        _mediator.Send(_registerCommand, Cancellation.Token).Returns(error);
        // When
        var result = await AuthenticationModule.RegisterAsync(_context, _registerRequest, Cancellation.Token);
        // Then
        result.Should().BeOfType<ProblemHttpResult>()
            .Which.ProblemDetails.Should().Match(x => x.Title == error.GetType().Name)
            .And.Match(x => x.Detail == error.Message);
    }

    [Fact]
    public async Task LoginAsync_OnFailure_ReturnsError()
    {
        // Given
        var error = Fixture.Create<DuplicateEmailError>();
        _mediator.Send(_authenticateQuery, Cancellation.Token).Returns(error);
        // When
        var result = await AuthenticationModule.LoginAsync(_context, _loginRequest, Cancellation.Token);
        // Then
        result.Should().BeOfType<ProblemHttpResult>()
            .Which.ProblemDetails.Should().Match(x => x.Title == error.GetType().Name)
            .And.Match(x => x.Detail == error.Message);
    }
}
