using AutoMapper;
using HomeInventory.Application;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Tests.DependencyInjection;

[UnitTest]
public class InfrastructureDependencyInjectionTests : BaseTest
{
    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly IServiceProviderFactory<IServiceCollection> _factory = new DefaultServiceProviderFactory();

    public InfrastructureDependencyInjectionTests()
    {
        _services.AddSingleton(Substitute.For<IIdFactory<UserId>>());
        _services.AddSingleton(Substitute.For<IHostEnvironment>());
        _services.AddSingleton(Substitute.For<IMapper>());
        _services.AddSingleton(Substitute.For<IDateTimeService>());
    }

    [Fact]
    public void ShouldRegister()
    {
        _services.AddInfrastructure();
        var provider = _factory.CreateServiceProvider(_services);

        _services.Should().ContainSingleScoped<IUserRepository>(provider);
        _services.Should().ContainSingleSingleton<IMappingAssemblySource>(provider);
    }
}
