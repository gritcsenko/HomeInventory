using HomeInventory.Application;
using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Domain.UserManagement.ValueObjects;
using HomeInventory.Tests.Architecture;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.Logging;

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
    public void Should_BeResolved()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(TestingLogger<>.Stub));

        var serviceConfig = new MediatRServiceConfiguration()
            .RegisterServicesFromAssemblies(AssemblyReferences.Application.Assembly)
            .AddLoggingBehavior();
        ServiceRegistrar.AddMediatRClasses(services, serviceConfig);
        ServiceRegistrar.AddRequiredServices(services, serviceConfig);

        var behavior = services.BuildServiceProvider().GetRequiredService<IPipelineBehavior<AuthenticateQuery, IQueryResult<AuthenticateResult>>>();

        behavior.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnResponseFromNext()
    {
        var sut = CreateSut();

        var response = await sut.Handle(_request, Handler, Cancellation.Token);

        response.Should().BeSameAs(_response);

        Task<IQueryResult<AuthenticateResult>> Handler(CancellationToken _) => Task.FromResult(_response);
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
