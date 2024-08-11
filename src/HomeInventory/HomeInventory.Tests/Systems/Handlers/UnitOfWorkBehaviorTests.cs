using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Messages;
using Microsoft.Extensions.Logging;
using AssemblyReference = HomeInventory.Application.AssemblyReference;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class UnitOfWorkBehaviorTests : BaseTest
{
    private readonly TestingLogger<UnitOfWorkRequestBehavior<RegisterUserRequestMessage, Option<Error>>> _logger = Substitute.For<TestingLogger<UnitOfWorkRequestBehavior<RegisterUserRequestMessage, Option<Error>>>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IRequestContext<RegisterUserRequestMessage> _context = Substitute.For<IRequestContext<RegisterUserRequestMessage>>();
    private readonly ScopeAccessor _scopeAccessor = new(new ScopeContainer(new ScopeFactory()));
    private readonly ServiceProvider _services;

    public UnitOfWorkBehaviorTests()
    {
        Fixture.CustomizeUlid();
        AddDisposable(_scopeAccessor.GetScope<IUnitOfWork>().Set(_unitOfWork));
        var services = new ServiceCollection();
        services.AddSingleton<IScopeAccessor>(_scopeAccessor);
        services.AddSingleton(typeof(ILogger<>), typeof(TestingLogger<>.Stub));
        services.AddDomain();
        services.AddMessageHub(
            HomeInventory.Application.AssemblyReference.Assembly,
            HomeInventory.Application.UserManagement.AssemblyReference.Assembly);
        _services = services.BuildServiceProvider();

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

    private UnitOfWorkRequestBehavior<RegisterUserRequestMessage, Option<Error>> CreateSut() => new(_scopeAccessor, _logger);
}
