using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Authentication;
using HomeInventory.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HomeInventory.Tests.DependencyInjection;

[Trait("Category", "Unit")]
public class InfrastructureDependencyInjectionTests : BaseTest
{
    private readonly IServiceCollection _services = new TestingServiceCollection();

    [Fact]
    public void ShouldRegister()
    {
        _services.AddInfrastructure();

        _services.Should().ContainSingleTransient<IConfigureOptions<JwtSettings>>();
        _services.Should().ContainSingleSingleton<IJwtIdentityGenerator>();
        _services.Should().ContainSingleSingleton<IAuthenticationTokenGenerator>();
        _services.Should().ContainSingleSingleton<IDateTimeService>();
        _services.Should().ContainSingleScoped<IUserRepository>();
        _services.Should().ContainSingleSingleton<IMappingAssemblySource>();
    }
}
