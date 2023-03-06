using AutoMapper;
using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Primitives;
using HomeInventory.Tests.Support;
using HomeInventory.Web;
using HomeInventory.Web.Authentication;
using HomeInventory.Web.Authorization.Dynamic;
using HomeInventory.Web.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using IConfigurationProvider = Microsoft.Extensions.Configuration.IConfigurationProvider;

namespace HomeInventory.Tests.DependencyInjection;

[Trait("Category", "Unit")]
public class WebDependencyInjectionTests : BaseTest
{
    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly IServiceProviderFactory<IServiceCollection> _factory = new DefaultServiceProviderFactory();

    public WebDependencyInjectionTests()
    {
        var providers = new List<IConfigurationProvider>{
            new MemoryConfigurationProvider(new MemoryConfigurationSource{
                InitialData = new Dictionary<string, string?>{
                    [$"{nameof(JwtOptions)}:{nameof(JwtOptions.Secret)}"] = "Some Secret",
                    [$"{nameof(JwtOptions)}:{nameof(JwtOptions.Issuer)}"] = "HomeInventory",
                    [$"{nameof(JwtOptions)}:{nameof(JwtOptions.Audience)}"] = "HomeInventory",
                },
            })
        };
        _services.AddSingleton<IConfiguration>(new ConfigurationRoot(providers));

        var env = Substitute.For<IWebHostEnvironment>();
        env.WebRootFileProvider.Returns(new NullFileProvider());
        _services.AddSingleton(env);
        _services.AddSingleton<IHostEnvironment>(env);
        _services.AddSingleton<IDateTimeService>(new FixedTestingDateTimeService { Now = DateTimeOffset.Now });
    }

    [Fact]
    public void ShouldRegister()
    {
        _services.AddWeb();
        var provider = _factory.CreateServiceProvider(_services);

        _services.Should().ContainSingleTransient<IConfigureOptions<JwtOptions>>(provider);
        _services.Should().ContainSingleTransient<IConfigureOptions<JwtBearerOptions>>(provider);
        _services.Should().ContainSingleSingleton<IJwtIdentityGenerator>(provider);
        _services.Should().ContainSingleSingleton<IAuthenticationTokenGenerator>(provider);
        _services.Should().ContainSingleSingleton<HealthCheckService>(provider);
        _services.Should().ContainSingleSingleton<ProblemDetailsFactory>(provider);
        _services.Should().ContainSingleTransient<IMapper>(provider);
        _services.Should().ContainSingleSingleton<IMappingAssemblySource>(provider);
        _services.Should().ContainSingleSingleton<IControllerFactory>(provider);
        _services.Should().ContainSingleTransient<ISwaggerProvider>(provider);

        _services.Should().ContainSingleSingleton<PermissionList>(provider);
        _services.Should().ContainTransient<IAuthorizationHandler>(provider);
        _services.Should().ContainSingleTransient<IAuthorizationService>(provider);
        _services.Should().ContainSingleTransient<IAuthorizationPolicyProvider>(provider);
        _services.Should().ContainSingleTransient<IAuthorizationHandlerProvider>(provider);
        _services.Should().ContainSingleTransient<IAuthorizationEvaluator>(provider);
        _services.Should().ContainSingleTransient<IAuthorizationHandlerContextFactory>(provider);
        _services.Should().ContainSingleTransient<IPolicyEvaluator>(provider);
        _services.Should().ContainSingleTransient<IAuthorizationMiddlewareResultHandler>(provider);

        var swaggerOptions = new SwaggerGenOptions();
        _services.Should().ContainSingleTransient<IConfigureOptions<SwaggerGenOptions>>(provider)
            .Which.Configure(swaggerOptions);
        swaggerOptions.SwaggerGeneratorOptions.SwaggerDocs.Should().ContainKey("v1")
            .WhoseValue.Version.Should().Be("1");
    }

    [Fact]
    public void ShouldUse()
    {
        _services.AddWeb();
        var appBuilder = new TestAppBuilder(_services, _factory);

        appBuilder.UseWeb();
    }
}
