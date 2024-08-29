using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Messages;
using Microsoft.Extensions.Logging;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class UnitOfWorkBehaviorTests : BaseTest
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IRequestContext<RegisterUserRequestMessage> _context = Substitute.For<IRequestContext<RegisterUserRequestMessage>>();
    private readonly ServiceProvider _services;

    public UnitOfWorkBehaviorTests()
    {
        Fixture.CustomizeUlid();
        var services = new ServiceCollection();
        services.AddLoggerSubstitute<UnitOfWorkRequestBehavior<RegisterUserRequestMessage, Option<Error>>>();
        services.AddLoggerSubstitute<LoggingRequestBehavior<RegisterUserRequestMessage, Option<Error>>>();
        services.AddDomain();
        services.AddMessageHub(HomeInventory.Application.AssemblyReference.Assembly);

        _services = services.BuildServiceProvider();
        AddAsyncDisposable(_services);
        AddDisposable(_services.GetRequiredService<IScopeAccessor>().Set(_unitOfWork));

        _context.Hub.Returns(call => _services.GetRequiredService<IMessageHub>());
        _context.RequestAborted.Returns(call => Cancellation.Token);
    }

    [Fact]
    public void Should_BeResolvedForCommand()
    {
        var behavior = _services.GetRequiredService<IRequestPipelineBehavior<RegisterUserRequestMessage, Option<Error>>>();

        behavior.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnResponseFromNext()
    {
        var sut = CreateSut();
        var _response = Option<Error>.None;

        var response = await sut.OnRequestAsync(_context, Handler);

        response.Should().BeNone();

        Task<Option<Error>> Handler(IRequestContext<RegisterUserRequestMessage> _) => Task.FromResult(_response);
    }

    [Fact]
    public async Task Handle_Should_CallSave_When_Success()
    {
        var sut = CreateSut();
        var _response = Option<Error>.None;

        _ = await sut.OnRequestAsync(_context, Handler);

        _ = _unitOfWork
            .Received(1)
            .SaveChangesAsync(Cancellation.Token);

        Task<Option<Error>> Handler(IRequestContext<RegisterUserRequestMessage> _) => Task.FromResult(_response);
    }

    [Fact]
    public async Task Handle_Should_NotCallSave_When_Error()
    {
        var sut = CreateSut();
        var _response = Option<Error>.Some(new NotFoundError(Fixture.Create<string>()));

        _ = await sut.OnRequestAsync(_context, Handler);

        _ = _unitOfWork
            .Received(0)
            .SaveChangesAsync(Cancellation.Token);

        Task<Option<Error>> Handler(IRequestContext<RegisterUserRequestMessage> _) => Task.FromResult(_response);
    }

    private IRequestPipelineBehavior<RegisterUserRequestMessage, Option<Error>> CreateSut() =>
        _services
            .GetServices<IRequestPipelineBehavior<RegisterUserRequestMessage, Option<Error>>>()
            .First(b => b is UnitOfWorkRequestBehavior<RegisterUserRequestMessage, Option<Error>>);
}
