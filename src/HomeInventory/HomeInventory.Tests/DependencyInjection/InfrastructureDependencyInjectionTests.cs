using AutoMapper;
using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Tests.DependencyInjection;

[UnitTest]
public class InfrastructureDependencyInjectionTests : BaseDependencyInjectionTest
{
    public InfrastructureDependencyInjectionTests()
    {
        Services.AddSingleton(Substitute.For<IIdFactory<UserId, Guid>>());
        Services.AddSingleton(Substitute.For<IHostEnvironment>());
        Services.AddSingleton(Substitute.For<IMapper>());
    }

    [Fact]
    public void ShouldRegister()
    {
        Services.AddInfrastructure();
        var provider = CreateProvider();

        Services.Should().ContainSingleScoped<IUserRepository>(provider);
        Services.Should().ContainSingleSingleton<IMappingAssemblySource>(provider);
    }
}
