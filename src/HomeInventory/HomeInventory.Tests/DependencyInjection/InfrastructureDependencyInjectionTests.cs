using AutoMapper;
using HomeInventory.Application;
using HomeInventory.Domain.Persistence;
using HomeInventory.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Tests.DependencyInjection;

[UnitTest]
public class InfrastructureDependencyInjectionTests : BaseDependencyInjectionTest
{
    public InfrastructureDependencyInjectionTests()
    {
        Services.AddSingleton(Substitute.For<IHostEnvironment>());
        Services.AddSingleton(Substitute.For<IMapper>());
        AddDateTime();
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
