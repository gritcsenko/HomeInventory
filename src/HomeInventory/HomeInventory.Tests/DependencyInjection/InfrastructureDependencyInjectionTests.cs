using AutoMapper;
using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure;
using HomeInventory.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace HomeInventory.Tests.DependencyInjection;

[Trait("Category", "Unit")]
public class InfrastructureDependencyInjectionTests : BaseTest
{
    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly IServiceProviderFactory<IServiceCollection> _factory = new DefaultServiceProviderFactory();

    public InfrastructureDependencyInjectionTests()
    {
        _services.AddSingleton(Substitute.For<IUserIdFactory>());
        _services.AddSingleton(Substitute.For<IHostEnvironment>());
        _services.AddSingleton(Substitute.For<IMapper>());
    }

    [Fact]
    public void ShouldRegister()
    {
        _services.AddInfrastructure();
        var provider = _factory.CreateServiceProvider(_services);

        _services.Should().ContainSingleSingleton<IDateTimeService>(provider);
        _services.Should().ContainSingleScoped<IUserRepository>(provider);
        _services.Should().ContainSingleSingleton<IMappingAssemblySource>(provider);
    }
}
