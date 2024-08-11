using HomeInventory.Application;
using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using AssemblyReference = HomeInventory.Application.AssemblyReference;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class LoggingBehaviorTests : BaseTest
{
    private readonly TestingLogger<LoggingRequestBehavior<AuthenticateRequestMessage, IQueryResult<AuthenticateResult>>> _logger = Substitute.For<TestingLogger<LoggingRequestBehavior<AuthenticateRequestMessage, IQueryResult<AuthenticateResult>>>>();
    private readonly IQueryResult<AuthenticateResult> _response = Substitute.For<IQueryResult<AuthenticateResult>>();
    private readonly IRequestContext<AuthenticateRequestMessage> _context = Substitute.For<IRequestContext<AuthenticateRequestMessage>>();
    private readonly ServiceProvider _services;

    public LoggingBehaviorTests()
    {
        Fixture.CustomizeId<UserId>();
        Fixture.CustomizeEmail();

        var services = new ServiceCollection();
        ////services.AddSingleton(typeof(ILogger<>), typeof(TestingLogger<>.Stub));
        services.AddSingleton<ILogger<LoggingRequestBehavior<AuthenticateRequestMessage, IQueryResult<AuthenticateResult>>>>(_logger);
        services.AddDomain();
        services.AddMessageHub(
            HomeInventory.Application.AssemblyReference.Assembly,
            HomeInventory.Application.UserManagement.AssemblyReference.Assembly);
        _services = services.BuildServiceProvider();

        _context.Hub.Returns(call => _services.GetRequiredService<IMessageHub>());
        _context.RequestAborted.Returns(call => Cancellation.Token);
    }

    [Fact]
    public void Should_BeResolved()
    {
        var sut = CreateSut();

        sut.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnResponseFromNext()
    {
        var sut = CreateSut();

        var response = await sut.OnRequestAsync(_context, Handler);

        response.Should().BeSameAs(_response);

        Task<IQueryResult<AuthenticateResult>> Handler(IRequestContext<AuthenticateRequestMessage> context)
        {
            return Task.FromResult(_response);
        }
    }

    [Fact]
    public async Task Handle_Should_LogBeforeCallingNext()
    {
        var sut = CreateSut();

        _ = await sut.OnRequestAsync(_context, Handler);

        Task<IQueryResult<AuthenticateResult>> Handler(IRequestContext<AuthenticateRequestMessage> context)
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
        _response.IsSuccess.Returns(true);

        _ = await sut.OnRequestAsync(_context, Handler);

        _logger
            .Received(1)
            .Log(LogLevel.Information, Arg.Any<EventId>(), Arg.Any<object>(), null, Arg.Any<Func<object, Exception?, string>>());

        Task<IQueryResult<AuthenticateResult>> Handler(IRequestContext<AuthenticateRequestMessage> context)
        {
            _logger.ClearReceivedCalls();
            return Task.FromResult(_response);
        }
    }

    private IRequestPipelineBehavior<AuthenticateRequestMessage, IQueryResult<AuthenticateResult>> CreateSut() => _services.GetRequiredService<IRequestPipelineBehavior<AuthenticateRequestMessage, IQueryResult<AuthenticateResult>>>();
}
