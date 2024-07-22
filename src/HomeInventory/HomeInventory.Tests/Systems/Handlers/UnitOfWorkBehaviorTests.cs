using HomeInventory.Application;
using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;
using AssemblyReference = HomeInventory.Application.AssemblyReference;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class UnitOfWorkBehaviorTests : BaseTest
{
    private readonly TestingLogger<UnitOfWorkBehavior<RegisterCommand, OneOf<Success, IError>>> _logger = Substitute.For<TestingLogger<UnitOfWorkBehavior<RegisterCommand, OneOf<Success, IError>>>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ScopeAccessor _scopeAccessor = new();

    public UnitOfWorkBehaviorTests()
    {
        Fixture.CustomizeUlid();
        AddDisposable(_scopeAccessor.GetScope<IUnitOfWork>().Set(_unitOfWork));
    }

    [Fact]
    public void Should_BeResolvedForCommand()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IScopeAccessor>(_scopeAccessor);
        services.AddSingleton(typeof(ILogger<>), typeof(TestingLogger<>.Stub));

        var serviceConfig = new MediatRServiceConfiguration()
            .RegisterServicesFromAssemblies(AssemblyReference.Assembly)
            .AddUnitOfWorkBehavior();
        ServiceRegistrar.AddMediatRClasses(services, serviceConfig);
        ServiceRegistrar.AddRequiredServices(services, serviceConfig);

        var behavior = services.BuildServiceProvider().GetRequiredService<IPipelineBehavior<RegisterCommand, OneOf<Success, IError>>>();

        behavior.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnResponseFromNext()
    {
        var sut = CreateSut();
        var _request = Fixture.Create<RegisterCommand>();
        var _response = OneOf<Success, IError>.FromT0(new Success());

        var response = await sut.Handle(_request, Handler, Cancellation.Token);

        response.Value.Should().Be(_response.Value);

        Task<OneOf<Success, IError>> Handler() => Task.FromResult(_response);
    }

    [Fact]
    public async Task Handle_Should_CallSave_When_Success()
    {
        var sut = CreateSut();
        var _request = Fixture.Create<RegisterCommand>();
        var _response = OneOf<Success, IError>.FromT0(new Success());

        _ = await sut.Handle(_request, Handler, Cancellation.Token);

        _ = _unitOfWork
            .Received(1)
            .SaveChangesAsync(Cancellation.Token);

        Task<OneOf<Success, IError>> Handler() => Task.FromResult(_response);
    }

    [Fact]
    public async Task Handle_Should_NotCallSave_When_Error()
    {
        var sut = CreateSut();
        var _request = Fixture.Create<RegisterCommand>();
        var _response = OneOf<Success, IError>.FromT1(new NotFoundError(Fixture.Create<string>()));

        _ = await sut.Handle(_request, Handler, Cancellation.Token);

        _ = _unitOfWork
            .Received(0)
            .SaveChangesAsync(Cancellation.Token);

        Task<OneOf<Success, IError>> Handler() => Task.FromResult(_response);
    }

    private UnitOfWorkBehavior<RegisterCommand, OneOf<Success, IError>> CreateSut() => new(_scopeAccessor, _logger);
}
