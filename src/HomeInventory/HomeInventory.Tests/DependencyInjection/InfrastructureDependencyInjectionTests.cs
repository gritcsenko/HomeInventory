using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Authentication;
using HomeInventory.Tests.Helpers;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace HomeInventory.Tests.DependencyInjection;

[Trait("Category", "Unit")]
public class InfrastructureDependencyInjectionTests : BaseTest
{
    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly IServiceProviderFactory<IServiceCollection> _factory = new DefaultServiceProviderFactory();
    private readonly IConfiguration _configuration;

    public InfrastructureDependencyInjectionTests()
    {
        var providers = new List<IConfigurationProvider>{
            new MemoryConfigurationProvider(new MemoryConfigurationSource{
                InitialData = new Dictionary<string, string?>{
                    ["JwtOptions:Secret"] = "Some Secret",
                },
            })
        };
        _configuration = new ConfigurationRoot(providers);
        _services.AddSingleton(_configuration);
        _services.AddSingleton(Substitute.For<IUserIdFactory>());
        _services.AddSingleton(Substitute.For<IHostEnvironment>());
        _services.AddSingleton(Substitute.For<IMapper>());
    }

    [Fact]
    public void ShouldRegister()
    {
        _services.AddInfrastructure(_configuration);
        var provider = _factory.CreateServiceProvider(_services);

        _services.Should().ContainSingleTransient<IConfigureOptions<JwtOptions>>(provider);
        _services.Should().ContainSingleTransient<IPostConfigureOptions<JwtBearerOptions>>(provider);
        _services.Should().ContainSingleSingleton<IJwtIdentityGenerator>(provider);
        _services.Should().ContainSingleSingleton<IAuthenticationTokenGenerator>(provider);
        _services.Should().ContainSingleSingleton<IDateTimeService>(provider);
        _services.Should().ContainSingleScoped<IUserRepository>(provider);
        _services.Should().ContainSingleSingleton<IMappingAssemblySource>(provider);
    }
}
