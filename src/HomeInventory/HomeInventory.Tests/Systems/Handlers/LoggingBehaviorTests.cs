using HomeInventory.Application;
using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using AssemblyReference = HomeInventory.Application.AssemblyReference;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class LoggingBehaviorTests : BaseTest
{
    private readonly TestingLogger<LoggingBehavior<AuthenticateQuery, IQueryResult<AuthenticateResult>>> _logger = Substitute.For<TestingLogger<LoggingBehavior<AuthenticateQuery, IQueryResult<AuthenticateResult>>>>();
    private readonly AuthenticateQuery _request;
    private readonly IQueryResult<AuthenticateResult> _response;

    public LoggingBehaviorTests()
    {
        Fixture.CustomizeId<UserId>();
        Fixture.CustomizeEmail();
        _request = Fixture.Create<AuthenticateQuery>();
        _response = Substitute.For<IQueryResult<AuthenticateResult>>();
    }

    [Fact]
    public async Task Handle_Should_ReturnResponseFromNext()
    {
        var sut = CreateSut();

        var response = await sut.Handle(_request, Handler, Cancellation.Token);

        response.Should().BeSameAs(_response);

        Task<IQueryResult<AuthenticateResult>> Handler(CancellationToken _)
        {
            return Task.FromResult(_response);
        }
    }

    [Fact]
    public async Task Handle_Should_LogBeforeCallingNext()
    {
        var sut = CreateSut();

        _ = await sut.Handle(_request, Handler, Cancellation.Token);

        Task<IQueryResult<AuthenticateResult>> Handler(CancellationToken _)
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

        Task<IQueryResult<AuthenticateResult>> Handler(CancellationToken _)
        {
            _logger.ClearReceivedCalls();
            return Task.FromResult(_response);
        }
    }

    private LoggingBehavior<AuthenticateQuery, IQueryResult<AuthenticateResult>> CreateSut() => new(_logger);
}
