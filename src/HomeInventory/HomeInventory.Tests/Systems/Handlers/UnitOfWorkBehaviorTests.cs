using HomeInventory.Application;
using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.Logging;
using AssemblyReference = HomeInventory.Application.AssemblyReference;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class UnitOfWorkBehaviorTests : BaseTest
{
    private readonly TestingLogger<UnitOfWorkBehavior<RegisterCommand, Option<Error>>> _logger = Substitute.For<TestingLogger<UnitOfWorkBehavior<RegisterCommand, Option<Error>>>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ScopeAccessor _scopeAccessor = new(new ScopeContainer(new ScopeFactory()));

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

        var actual = services.BuildServiceProvider().GetRequiredService<IPipelineBehavior<RegisterCommand, Option<Error>>>();

        actual.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnResponseFromNext()
    {
        var sut = CreateSut();
        var request = Fixture.Create<RegisterCommand>();
        var response = Option<Error>.None;

        var actual = await sut.Handle(request, Handler, Cancellation.Token);

        actual.Should().BeNone();

        Task<Option<Error>> Handler(CancellationToken _) => Task.FromResult(response);
    }

    [Fact]
    public async Task Handle_Should_CallSave_When_Success()
    {
        var sut = CreateSut();
        var request = Fixture.Create<RegisterCommand>();
        var response = Option<Error>.None;

        _ = await sut.Handle(request, Handler, Cancellation.Token);

        _ = _unitOfWork
            .Received(1)
            .SaveChangesAsync(Cancellation.Token);

        Task<Option<Error>> Handler(CancellationToken _) => Task.FromResult(response);
    }

    [Fact]
    public async Task Handle_Should_NotCallSave_When_Error()
    {
        var sut = CreateSut();
        var request = Fixture.Create<RegisterCommand>();
        var response = Option<Error>.Some(new NotFoundError(Fixture.Create<string>()));

        _ = await sut.Handle(request, Handler, Cancellation.Token);

        _ = _unitOfWork
            .Received(0)
            .SaveChangesAsync(Cancellation.Token);

        Task<Option<Error>> Handler(CancellationToken _) => Task.FromResult(response);
    }

    private UnitOfWorkBehavior<RegisterCommand, Option<Error>> CreateSut() => new(_scopeAccessor, _logger);
}
