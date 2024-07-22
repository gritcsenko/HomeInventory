using HomeInventory.Application;
using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using OneOf;
using AssemblyReference = HomeInventory.Application.AssemblyReference;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class LoggingBehaviorTests : BaseTest
{
    private readonly TestingLogger<LoggingRequestBehavior<AuthenticateRequestMessage, AuthenticateResult>> _logger = Substitute.For<TestingLogger<LoggingRequestBehavior<AuthenticateRequestMessage, AuthenticateResult>>>();
    private readonly AuthenticateRequestMessage _request;
    private readonly OneOf<AuthenticateResult, IError> _response;
    private readonly ServiceProvider _services;

    public LoggingBehaviorTests()
    {
        Fixture.CustomizeId<UserId>();
        Fixture.CustomizeEmail();
        _request = Fixture.Create<AuthenticateRequestMessage>();
        _response = Fixture.Create<AuthenticateResult>();
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(TestingLogger<>.Stub));
        services.AddDomain();
        services.AddMessageHub(AssemblyReference.Assembly);
        _services = services.BuildServiceProvider();
    }

    [Fact]
    public void Should_BeResolved()
    {
        var behavior = _services.GetRequiredService<IRequestPipelineBehavior<AuthenticateRequestMessage, AuthenticateResult>>();

        behavior.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnResponseFromNext()
    {
        var sut = CreateSut();

        var response = await sut.OnRequest(Hub, _request, Handler, Cancellation.Token);

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

        _ = await sut.OnRequest(Hub, _request, Handler, Cancellation.Token);

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

        _ = await sut.OnRequest(Hub, _request, Handler, Cancellation.Token);

        _logger
            .Received(1)
            .Log(LogLevel.Information, Arg.Any<EventId>(), Arg.Any<object>(), null, Arg.Any<Func<object, Exception?, string>>());

        Task<OneOf<AuthenticateResult, IError>> Handler()
        {
            _logger.ClearReceivedCalls();
            return Task.FromResult(_response);
        }
    }

    private IMessageHub Hub => _services.GetRequiredService<IMessageHub>();

    private LoggingRequestBehavior<AuthenticateRequestMessage, AuthenticateResult> CreateSut() => new(_logger);
}
