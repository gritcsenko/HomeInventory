using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Modules;
using HomeInventory.Web;
using HomeInventory.Web.Authorization.Dynamic;
using HomeInventory.Web.ErrorHandling;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Framework.Infrastructure;
using HomeInventory.Web.OpenApi;
using HomeInventory.Web.UserManagement;
using HomeInventory.Web.UserManagement.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using ContractsMapper = HomeInventory.Web.ContractsMapper;

namespace HomeInventory.Tests.DependencyInjection;

[UnitTest]
public class WebDependencyInjectionTests : BaseDependencyInjectionTest
{
    private readonly ModulesHost _host;
    private readonly IConfiguration _configuration;
    private readonly IMetricsBuilder _metricsBuilder = Substitute.For<IMetricsBuilder>();

    public WebDependencyInjectionTests()
    {
        _configuration = AddConfiguration(new Dictionary<string, string?>
        {
            [JwtOptions.Section / nameof(JwtOptions.Secret)] = "Some Secret",
            [JwtOptions.Section / nameof(JwtOptions.Issuer)] = "HomeInventory",
            [JwtOptions.Section / nameof(JwtOptions.Audience)] = "HomeInventory",
        });
        AddDateTime();

        var env = Substitute.For<IWebHostEnvironment>();
        env.WebRootFileProvider.Returns(new NullFileProvider());
        Services.AddSingleton(env);
        Services.AddSingleton<IHostEnvironment>(env);

        _host = new([
            new WebCarterSupportModule(),
            new WebUserManagementModule(),
            new WebHealthCheckModule(),
            new WebErrorHandlingModule(),
            new WebSwaggerModule(),
            new DynamicWebAuthorizationModule(),
        ]);
    }

    [Fact]
    public async Task ShouldRegister()
    {
        await _host.AddServicesAsync(Services, _configuration, _metricsBuilder);

        using var scope = new AssertionScope();
        Services.Should().ContainConfigureOptions<JwtOptions>();
        Services.Should().ContainConfigureOptions<JwtBearerOptions>();
        Services.Should().ContainSingleSingleton<IJwtIdentityGenerator>();
        Services.Should().ContainSingleScoped<IAuthenticationTokenGenerator>();
        Services.Should().ContainSingleSingleton<HealthCheckService>();
        Services.Should().ContainSingleTransient<HomeInventoryProblemDetailsFactory>();
        Services.Should().ContainSingleTransient<ProblemDetailsFactory>();
        Services.Should().ContainSingleTransient<IProblemDetailsFactory>();
        Services.Should().ContainSingleTransient<ISwaggerProvider>();
        Services.Should().ContainSingleSingleton<PermissionList>();
        Services.Should().ContainTransient<IAuthorizationHandler>();
        Services.Should().ContainSingleTransient<IAuthorizationService>();
        Services.Should().ContainSingleTransient<IAuthorizationPolicyProvider>();
        Services.Should().ContainSingleTransient<IAuthorizationHandlerProvider>();
        Services.Should().ContainSingleTransient<IAuthorizationEvaluator>();
        Services.Should().ContainSingleTransient<IAuthorizationHandlerContextFactory>();
        Services.Should().ContainSingleTransient<IPolicyEvaluator>();
        Services.Should().ContainSingleTransient<IAuthorizationMiddlewareResultHandler>();
        Services.Should().ContainSingleTransient<ContractsMapper>();

        var provider = CreateProvider();
        var swaggerOptions = new SwaggerGenOptions();
        Services.Should().ContainConfigureOptions<SwaggerGenOptions>(provider)
            .Which.Configure(swaggerOptions);
        swaggerOptions.SwaggerGeneratorOptions.SwaggerDocs.Should().ContainKey("v1")
            .WhoseValue.Version.Should().Be("1");
    }
}
