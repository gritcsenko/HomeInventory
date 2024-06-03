using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Messages;
using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;
using AssemblyReference = HomeInventory.Application.AssemblyReference;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class UnitOfWorkBehaviorTests : BaseTest
{
    private readonly TestingLogger<UnitOfWorkRequestBehavior<RegisterUserRequestMessage>> _logger = Substitute.For<TestingLogger<UnitOfWorkRequestBehavior<RegisterUserRequestMessage>>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ScopeAccessor _scopeAccessor = new();
    private readonly ServiceProvider _services;

    public UnitOfWorkBehaviorTests()
    {
        Fixture.CustomizeCuid();
        AddDisposable(_scopeAccessor.GetScope<IUnitOfWork>().Set(_unitOfWork));
        var services = new ServiceCollection();
        services.AddSingleton<IScopeAccessor>(_scopeAccessor);
        services.AddSingleton(typeof(ILogger<>), typeof(TestingLogger<>.Stub));
        services.AddMessageHub(AssemblyReference.Assembly);
        _services = services.BuildServiceProvider();
    }

    [Fact]
    public void Should_BeResolvedForCommand()
    {
        var behavior = _services.GetRequiredService<IRequestPipelineBehavior<RegisterUserRequestMessage, Success>>();

        behavior.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnResponseFromNext()
    {
        var sut = CreateSut();
        var _request = Fixture.Create<RegisterUserRequestMessage>();
        var _response = OneOf<Success, IError>.FromT0(new Success());

        var response = await sut.OnRequest(Hub, _request, Handler, Cancellation.Token);

        response.Value.Should().Be(_response.Value);

        Task<OneOf<Success, IError>> Handler() => Task.FromResult(_response);
    }

    [Fact]
    public async Task Handle_Should_CallSave_When_Success()
    {
        var sut = CreateSut();
        var _request = Fixture.Create<RegisterUserRequestMessage>();
        var _response = OneOf<Success, IError>.FromT0(new Success());

        _ = await sut.OnRequest(Hub, _request, Handler, Cancellation.Token);

        _ = _unitOfWork
            .Received(1)
            .SaveChangesAsync(Cancellation.Token);

        Task<OneOf<Success, IError>> Handler() => Task.FromResult(_response);
    }

    [Fact]
    public async Task Handle_Should_NotCallSave_When_Error()
    {
        var sut = CreateSut();
        var _request = Fixture.Create<RegisterUserRequestMessage>();
        var _response = OneOf<Success, IError>.FromT1(new NotFoundError(Fixture.Create<string>()));

        _ = await sut.OnRequest(Hub, _request, Handler, Cancellation.Token);

        _ = _unitOfWork
            .Received(0)
            .SaveChangesAsync(Cancellation.Token);

        Task<OneOf<Success, IError>> Handler() => Task.FromResult(_response);
    }

    private IMessageHub Hub => _services.GetRequiredService<IMessageHub>();

    private UnitOfWorkRequestBehavior<RegisterUserRequestMessage> CreateSut() => new(_scopeAccessor, _logger);
}
