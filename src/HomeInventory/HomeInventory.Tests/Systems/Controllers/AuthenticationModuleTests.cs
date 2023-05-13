using AutoMapper;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Contracts;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Web.Infrastructure;
using HomeInventory.Web.Modules;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OneOf;

namespace HomeInventory.Tests.Systems.Controllers;

[UnitTest]
public class AuthenticationModuleTests : BaseTest<AuthenticationModuleTests.GivenTestContext>
{
    private static readonly Variable<RegistrationResult> _registrationResult = new(nameof(_registrationResult));
    private static readonly Variable<RegisterResponse> _registerResponse = new(nameof(_registerResponse));
    private static readonly Variable<RegisterRequest> _registerRequest = new(nameof(_registerRequest));
    private static readonly Variable<RegisterCommand> _registerCommand = new(nameof(_registerCommand));

    private static readonly Variable<AuthenticateResult> _authenticateResult = new(nameof(_authenticateResult));
    private static readonly Variable<LoginResponse> _loginResponse = new(nameof(_loginResponse));
    private static readonly Variable<LoginRequest> _loginRequest = new(nameof(_loginRequest));
    private static readonly Variable<AuthenticateQuery> _authenticateQuery = new(nameof(_authenticateQuery));

    private static readonly Variable<DuplicateEmailError> _error = new(nameof(_error));

    public AuthenticationModuleTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();
    }

    [Fact]
    public async Task RegisterAsync_OnSuccess_ReturnsHttp200()
    {
        Given
            .Map(_registerRequest, _registerCommand)
            .Map(_registrationResult, _registerResponse)
            .OnSendReturn(_registerCommand, _registrationResult);

        var then = await When
            .InvokedAsync(Given.Context, _registerRequest, AuthenticationModule.RegisterAsync);

        then
            .Result(_registerResponse, (actual, expected) =>
                actual.Should().BeOfType<Ok<RegisterResponse>>()
                    .Which.Should().HaveValue(expected));
    }

    [Fact]
    public async Task LoginAsync_OnSuccess_ReturnsHttp200()
    {
        Given
            .Map(_loginRequest, _authenticateQuery)
            .Map(_authenticateResult, _loginResponse)
            .OnSendReturn(_authenticateQuery, _authenticateResult);

        var then = await When
            .InvokedAsync(Given.Context, _loginRequest, AuthenticationModule.LoginAsync);

        then
            .Result(_loginResponse, (actual, expected) =>
                actual.Should().BeOfType<Ok<LoginResponse>>()
                    .Which.Should().HaveValue(expected));
    }

    [Fact]
    public async Task RegisterAsync_OnFailure_ReturnsError()
    {
        Given
            .Map(_registerRequest, _registerCommand)
            .New(_error)
            .OnSendReturn(_registerCommand, _error);

        var then = await When
            .InvokedAsync(Given.Context, _registerRequest, AuthenticationModule.RegisterAsync);

        then
            .Result(_error, (actual, error) =>
                actual.Should().BeOfType<ProblemHttpResult>()
                    .Which.ProblemDetails.Should().Match(x => x.Title == error.GetType().Name)
                    .And.Match(x => x.Detail == error.Message));
    }

    [Fact]
    public async Task LoginAsync_OnFailure_ReturnsError()
    {
        Given
            .Map(_loginRequest, _authenticateQuery)
            .New(_error)
            .OnSendReturn(_authenticateQuery, _error);

        var then = await When
            .InvokedAsync(Given.Context, _loginRequest, AuthenticationModule.LoginAsync);

        then
            .Result(_error, (actual, error) =>
                actual.Should().BeOfType<ProblemHttpResult>()
                    .Which.ProblemDetails.Should().Match(x => x.Title == error.GetType().Name)
                    .And.Match(x => x.Detail == error.Message));
    }

    protected override GivenTestContext CreateGiven(VariablesCollection variables) =>
        new(variables, Fixture, Cancellation);

    public sealed class GivenTestContext : GivenContext<GivenTestContext>
    {
        private static readonly Variable<HttpContext> _context = new(nameof(_context));
        private readonly ISender _mediator = Substitute.For<ISender>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly ICancellation _cancellation;

        public GivenTestContext(VariablesCollection variables, IFixture fixture, ICancellation cancellation)
            : base(variables, fixture)
        {
            _cancellation = cancellation;

            var collection = new ServiceCollection();
            collection.AddSingleton(_mediator);
            collection.AddSingleton(_mapper);
            collection.AddSingleton(new HomeInventoryProblemDetailsFactory(Options.Create(new ApiBehaviorOptions())));

            Add(_context, () => new DefaultHttpContext
            {
                RequestServices = collection.BuildServiceProvider()
            });
        }

        public IndexedVariable<HttpContext> Context => _context.WithIndex(0);

        public GivenTestContext OnSendReturn<TRequest, TResult>(Variable<TRequest> request, Variable<TResult> result)
            where TRequest : notnull, IRequest<OneOf<TResult, IError>>
            where TResult : notnull
        {
            var requestValue = Variables.Get(request.WithIndex(0));
            var resultValue = Variables.Get(result.WithIndex(0));
            _mediator.Send(requestValue, _cancellation.Token).Returns(resultValue);
            return this;
        }

        public GivenTestContext OnSendReturn(Variable<RegisterCommand> request, Variable<DuplicateEmailError> result)
        {
            var requestValue = Variables.Get(request.WithIndex(0));
            var resultValue = Variables.Get(result.WithIndex(0));
            _mediator.Send(requestValue, _cancellation.Token).Returns(resultValue);
            return this;
        }

        public GivenTestContext OnSendReturn(Variable<AuthenticateQuery> request, Variable<DuplicateEmailError> result)
        {
            var requestValue = Variables.Get(request.WithIndex(0));
            var resultValue = Variables.Get(result.WithIndex(0));
            _mediator.Send(requestValue, _cancellation.Token).Returns(resultValue);
            return this;
        }

        public GivenTestContext Map<TSource, TDestination>(Variable<TSource> source, Variable<TDestination> destination)
            where TSource : notnull
            where TDestination : notnull
        {
            New(source);
            New(destination);
            _mapper.Map<TDestination>(Variables.Get(source.WithIndex(0)))
                .Returns(Variables.Get(destination.WithIndex(0)));
            return this;
        }
    }
}
