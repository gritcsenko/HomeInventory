using HomeInventory.Application;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class UnitOfWorkBehaviorTests : BaseTest
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public void Should()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_unitOfWork);
        services.AddSingleton(typeof(ILogger<>), typeof(TestingLogger<>.Stub));

        var serviceConfig = new MediatRServiceConfiguration()
            .RegisterServicesFromAssemblies(AssemblyReference.Assembly)
            .AddUnitOfWorkBehavior();
        ServiceRegistrar.AddMediatRClasses(services, serviceConfig);
        ServiceRegistrar.AddRequiredServices(services, serviceConfig);

        var behavior = services.BuildServiceProvider().GetRequiredService<IPipelineBehavior<RegisterCommand, OneOf<Success, IError>>>();

        behavior.Should().NotBeNull();
    }
}
