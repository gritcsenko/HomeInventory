using AutoMapper;
using FluentAssertions.Execution;
using HomeInventory.Application.Framework;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Modules;
using HomeInventory.Web.Authentication;
using HomeInventory.Web.Authorization.Dynamic;
using HomeInventory.Web.Configuration;
using HomeInventory.Web.ErrorHandling;
using HomeInventory.Web.Framework.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HomeInventory.Tests.DependencyInjection;

[UnitTest]
public class WebDependencyInjectionTests : BaseDependencyInjectionTest
{
    private readonly ModulesHost _host;
    private readonly IConfiguration _configuration;

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

        _host = new([]);
    }

    [Fact]
    public async Task ShouldRegister()
    {
        await _host.AddModulesAsync(Services, _configuration);

        using var scope = new AssertionScope();
        Services.Should().ContainConfigureOptions<JwtOptions>();
        Services.Should().ContainConfigureOptions<JwtBearerOptions>();
        Services.Should().ContainSingleSingleton<IJwtIdentityGenerator>();
        Services.Should().ContainSingleScoped<IAuthenticationTokenGenerator>();
        Services.Should().ContainSingleSingleton<HealthCheckService>();
        Services.Should().ContainSingleTransient<HomeInventoryProblemDetailsFactory>();
        Services.Should().ContainSingleTransient<ProblemDetailsFactory>();
        Services.Should().ContainSingleTransient<IProblemDetailsFactory>();
        Services.Should().ContainSingleTransient<IMapper>();
        Services.Should().ContainSingleton<IMappingAssemblySource>();
        Services.Should().ContainSingleSingleton<IControllerFactory>();
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

        var provider = CreateProvider();
        var swaggerOptions = new SwaggerGenOptions();
        Services.Should().ContainConfigureOptions<SwaggerGenOptions>(provider)
            .Which.Configure(swaggerOptions);
        swaggerOptions.SwaggerGeneratorOptions.SwaggerDocs.Should().ContainKey("v1")
            .WhoseValue.Version.Should().Be("1");
    }
}
