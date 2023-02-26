using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using OneOf;

namespace HomeInventory.Tests.Systems.Handlers;

[Trait("Category", "Unit")]
public class LoggingBehaviorTests : BaseTest
{
    private readonly TestingLogger<LoggingBehavior<AuthenticateQuery, OneOf<AuthenticateResult, IError>>> _logger;
    private readonly AuthenticateQuery _request;
    private readonly OneOf<AuthenticateResult, IError> _response;

    public LoggingBehaviorTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();
        _logger = Substitute.For<TestingLogger<LoggingBehavior<AuthenticateQuery, OneOf<AuthenticateResult, IError>>>>();
        _request = Fixture.Create<AuthenticateQuery>();
        _response = Fixture.Create<OneOf<AuthenticateResult, IError>>();
    }

    [Fact]
    public async Task Handle_Should_ReturnResponseFromNext()
    {
        var sut = CreateSut();

        var response = await sut.Handle(_request, () => Task.FromResult<IOneOf>(_response), CancellationToken);

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
            return Task.FromResult<IOneOf>(_response);
        }, CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_LogAfterCallingNext()
    {
        var sut = CreateSut();

        _ = await sut.Handle(_request, () =>
        {
            _logger.ClearReceivedCalls();
            return Task.FromResult<IOneOf>(_response);
        }, CancellationToken);

        _logger
            .Received(1)
            .Log(LogLevel.Information, new EventId(0), Arg.Any<object>(), null, Arg.Any<Func<object, Exception?, string>>());
    }

    private LoggingBehavior<AuthenticateQuery, OneOf<AuthenticateResult, IError>> CreateSut() => new(_logger);
}
