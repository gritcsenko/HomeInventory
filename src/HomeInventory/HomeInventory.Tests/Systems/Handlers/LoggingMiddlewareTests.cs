using HomeInventory.Application;
using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Domain.UserManagement.ValueObjects;
using Microsoft.Extensions.Logging;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class LoggingMiddlewareTests : BaseTest
{
    private readonly TestingLogger<LoggingMiddleware<UserIdQuery, IQueryResult<UserIdResult>>> _logger = Substitute.For<TestingLogger<LoggingMiddleware<UserIdQuery, IQueryResult<UserIdResult>>>>();
    private readonly UserIdQuery _request;
    private readonly IQueryResult<UserIdResult> _response;

    public LoggingMiddlewareTests()
    {
        Fixture.CustomizeId<UserId>();
        Fixture.CustomizeEmail();
        _request = Fixture.Create<UserIdQuery>();
        _response = Substitute.For<IQueryResult<UserIdResult>>();
    }

    [Fact]
    public void Should_BeResolved()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(TestingLogger<>.Stub));
        services.AddSingleton(typeof(LoggingMiddleware<,>));

        var behavior = services.BuildServiceProvider().GetRequiredService<LoggingMiddleware<UserIdQuery, IQueryResult<UserIdResult>>>();

        behavior.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnResponseFromNext()
    {
        var sut = CreateSut();

        var response = await sut.Handle(_request, Handler, Cancellation.Token);

        response.Should().BeSameAs(_response);

        Task<IQueryResult<UserIdResult>> Handler(CancellationToken _) => Task.FromResult(_response);
    }

    [Fact]
    public async Task Handle_Should_LogBeforeCallingNext()
    {
        var sut = CreateSut();

        _ = await sut.Handle(_request, Handler, Cancellation.Token);

        Task<IQueryResult<UserIdResult>> Handler(CancellationToken _)
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

        _ = await sut.Handle(_request, Handler, Cancellation.Token);

        _logger
            .Received(1)
            .Log(LogLevel.Information, Arg.Any<EventId>(), Arg.Any<object>(), null, Arg.Any<Func<object, Exception?, string>>());

        Task<IQueryResult<UserIdResult>> Handler(CancellationToken _)
        {
            _logger.ClearReceivedCalls();
            return Task.FromResult(_response);
        }
    }

    private LoggingMiddleware<UserIdQuery, IQueryResult<UserIdResult>> CreateSut() => new(_logger);
}
