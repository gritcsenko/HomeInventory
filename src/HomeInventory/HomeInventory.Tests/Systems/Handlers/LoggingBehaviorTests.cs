using HomeInventory.Application;
using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneOf;
using AssemblyReference = HomeInventory.Application.AssemblyReference;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class LoggingBehaviorTests : BaseTest
{
    private readonly TestingLogger<LoggingBehavior<AuthenticateQuery, OneOf<AuthenticateResult, IError>>> _logger = Substitute.For<TestingLogger<LoggingBehavior<AuthenticateQuery, OneOf<AuthenticateResult, IError>>>>();
    private readonly AuthenticateQuery _request;
    private readonly OneOf<AuthenticateResult, IError> _response;

    public LoggingBehaviorTests()
    {
        Fixture.CustomizeUlidId<UserId>();
        Fixture.CustomizeEmail();
        _request = Fixture.Create<AuthenticateQuery>();
        _response = Fixture.Create<AuthenticateResult>();
    }

    [Fact]
    public void Should_BeResolved()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(TestingLogger<>.Stub));

        var serviceConfig = new MediatRServiceConfiguration()
            .RegisterServicesFromAssemblies(AssemblyReference.Assembly)
            .AddLoggingBehavior();
        ServiceRegistrar.AddMediatRClasses(services, serviceConfig);
        ServiceRegistrar.AddRequiredServices(services, serviceConfig);

        var behavior = services.BuildServiceProvider().GetRequiredService<IPipelineBehavior<AuthenticateQuery, OneOf<AuthenticateResult, IError>>>();

        behavior.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnResponseFromNext()
    {
        var sut = CreateSut();

        var response = await sut.Handle(_request, Handler, Cancellation.Token);

        response.Value.Should().Be(_response.Value);

        Task<OneOf<AuthenticateResult, IError>> Handler()
        {
            return Task.FromResult(_response);
        }
    }

    [Fact]
    public async Task Handle_Should_LogBeforeCallingNext()
    {
        var sut = CreateSut();

        _ = await sut.Handle(_request, Handler, Cancellation.Token);

        Task<OneOf<AuthenticateResult, IError>> Handler()
        {
            _logger
                .Received(1)
                .Log(LogLevel.Information, LogEvents._sendingRequest, Arg.Any<object>(), null, Arg.Any<Func<object, Exception?, string>>());
            return Task.FromResult(_response);
        }
    }

    [Fact]
    public async Task Handle_Should_LogAfterCallingNext()
    {
        var sut = CreateSut();

        _ = await sut.Handle(_request, Handler, Cancellation.Token);

        _logger
            .Received(1)
            .Log(LogLevel.Information, Arg.Any<EventId>(), Arg.Any<object>(), null, Arg.Any<Func<object, Exception?, string>>());

        Task<OneOf<AuthenticateResult, IError>> Handler()
        {
            _logger.ClearReceivedCalls();
            return Task.FromResult(_response);
        }
    }

    private LoggingBehavior<AuthenticateQuery, OneOf<AuthenticateResult, IError>> CreateSut() => new(_logger);
}
