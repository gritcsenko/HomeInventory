using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Authentication;
using HomeInventory.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace HomeInventory.Tests.DependencyInjection;

[Trait("Category", "Unit")]
public class InfrastructureDependencyInjectionTests : BaseTest
{
    private readonly IServiceCollection _services = new TestingServiceCollection();
    private readonly IConfiguration _configuration;

    public InfrastructureDependencyInjectionTests()
    {
        _configuration = Substitute.For<IConfiguration>();
    }

    [Fact]
    public void ShouldRegister()
    {
        _services.AddInfrastructure(_configuration);

        _services.Should().ContainSingleSingleton<IOptions<JwtOptions>>();
        _services.Should().ContainSingleSingleton<IJwtIdentityGenerator>();
        _services.Should().ContainSingleSingleton<IAuthenticationTokenGenerator>();
        _services.Should().ContainSingleSingleton<IDateTimeService>();
        _services.Should().ContainSingleScoped<IUserRepository>();
        _services.Should().ContainSingleSingleton<IMappingAssemblySource>();
    }
}
