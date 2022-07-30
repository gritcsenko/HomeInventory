using AutoFixture;
using ErrorOr;
using FluentAssertions;
using HomeInventory.Application.Authentication.Behaviors;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Tests.Customizations;
using HomeInventory.Tests.Helpers;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HomeInventory.Tests.Systems.Handlers;

[Trait("Category", "Unit")]
public class LoggingBehaviorTests : BaseTest
{
    private readonly TestingLogger<LoggingBehavior<AuthenticateQuery, ErrorOr<AuthenticateResult>>> _logger;
    private readonly AuthenticateQuery _request;
    private readonly ErrorOr<AuthenticateResult> _response;

    public LoggingBehaviorTests()
    {
        Fixture.Customize(new UserIdCustomization());
        _logger = Substitute.For<TestingLogger<LoggingBehavior<AuthenticateQuery, ErrorOr<AuthenticateResult>>>>();
        _request = Fixture.Create<AuthenticateQuery>();
        _response = Fixture.Create<AuthenticateResult>();
    }

    [Fact]
    public async Task Handle_Should_ReturnResponseFromNext()
    {
        var sut = CreateSut();

        var response = await sut.Handle(_request, CancellationToken, () => Task.FromResult(_response));

        response.Value.Should().Be(_response.Value);
    }

    [Fact]
    public async Task Handle_Should_LogBeforeCallingNext()
    {
        var sut = CreateSut();

        _ = await sut.Handle(_request, CancellationToken, () =>
        {
            _logger
                .Received(1)
                .Log(LogLevel.Information, new EventId(0), Arg.Any<object>(), null, Arg.Any<Func<object, Exception?, string>>());
            return Task.FromResult(_response);
        });
    }

    [Fact]
    public async Task Handle_Should_LogAfterCallingNext()
    {
        var sut = CreateSut();

        _ = await sut.Handle(_request, CancellationToken, () =>
        {
            _logger.ClearReceivedCalls();
            return Task.FromResult(_response);
        });

        _logger
            .Received(1)
            .Log(LogLevel.Information, new EventId(0), Arg.Any<object>(), null, Arg.Any<Func<object, Exception?, string>>());
    }

    private LoggingBehavior<AuthenticateQuery, ErrorOr<AuthenticateResult>> CreateSut() => new(_logger);
}
