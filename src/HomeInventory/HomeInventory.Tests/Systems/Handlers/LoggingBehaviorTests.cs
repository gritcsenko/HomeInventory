using AutoFixture;
using FluentAssertions;
using FluentResults;
using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Helpers;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HomeInventory.Tests.Systems.Handlers;

[Trait("Category", "Unit")]
public class LoggingBehaviorTests : BaseTest
{
    private readonly TestingLogger<LoggingBehavior<AuthenticateQuery, AuthenticateResult>> _logger;
    private readonly AuthenticateQuery _request;
    private readonly IResult<AuthenticateResult> _response;

    public LoggingBehaviorTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();
        _logger = Substitute.For<TestingLogger<LoggingBehavior<AuthenticateQuery, AuthenticateResult>>>();
        _request = Fixture.Create<AuthenticateQuery>();
        _response = Result.Ok(Fixture.Create<AuthenticateResult>());
    }

    [Fact]
    public async Task Handle_Should_ReturnResponseFromNext()
    {
        var sut = CreateSut();

        var response = await sut.Handle(_request, () => Task.FromResult(_response), CancellationToken);

        response.Value.Should().Be(_response.Value);
    }

    [Fact]
    public async Task Handle_Should_LogBeforeCallingNext()
    {
        var sut = CreateSut();

        _ = await sut.Handle(_request, () =>
        {
            _logger
                .Received(1)
                .Log(LogLevel.Information, new EventId(0), Arg.Any<object>(), null, Arg.Any<Func<object, Exception?, string>>());
            return Task.FromResult(_response);
        }, CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_LogAfterCallingNext()
    {
        var sut = CreateSut();

        _ = await sut.Handle(_request, () =>
        {
            _logger.ClearReceivedCalls();
            return Task.FromResult(_response);
        }, CancellationToken);

        _logger
            .Received(1)
            .Log(LogLevel.Information, new EventId(0), Arg.Any<object>(), null, Arg.Any<Func<object, Exception?, string>>());
    }

    private LoggingBehavior<AuthenticateQuery, AuthenticateResult> CreateSut() => new(_logger);
}
