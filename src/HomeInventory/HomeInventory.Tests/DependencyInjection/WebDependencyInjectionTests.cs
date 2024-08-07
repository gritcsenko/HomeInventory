﻿using AutoMapper;
using FluentAssertions.Execution;
using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Web.Authentication;
using HomeInventory.Web.Authorization.Dynamic;
using HomeInventory.Web.Configuration;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HomeInventory.Tests.DependencyInjection;

[UnitTest]
public class WebDependencyInjectionTests : BaseDependencyInjectionTest
{
    public WebDependencyInjectionTests()
    {
        AddConfiguration(new Dictionary<string, string?>
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
    }

    [Fact]
    public void ShouldRegister()
    {
        Services.AddWeb(
            Web.AssemblyReference.Assembly,
            Web.UserManagement.AssemblyReference.Assembly,
            Contracts.Validations.AssemblyReference.Assembly,
            Contracts.UserManagement.Validators.AssemblyReference.Assembly);
        var provider = CreateProvider();

        using var scope = new AssertionScope();
        Services.Should().ContainConfigureOptions<JwtOptions>(provider);
        Services.Should().ContainConfigureOptions<JwtBearerOptions>(provider);
        Services.Should().ContainSingleSingleton<IJwtIdentityGenerator>(provider);
        Services.Should().ContainSingleScoped<IAuthenticationTokenGenerator>(provider);
        Services.Should().ContainSingleSingleton<HealthCheckService>(provider);
        Services.Should().ContainSingleTransient<HomeInventoryProblemDetailsFactory>(provider);
        Services.Should().ContainSingleTransient<ProblemDetailsFactory>(provider);
        Services.Should().ContainSingleTransient<IProblemDetailsFactory>(provider);
        Services.Should().ContainSingleTransient<IMapper>(provider);
        Services.Should().ContainSingleSingleton<IMappingAssemblySource>(provider);
        Services.Should().ContainSingleSingleton<IControllerFactory>(provider);
        Services.Should().ContainSingleTransient<ISwaggerProvider>(provider);
        Services.Should().ContainSingleSingleton<PermissionList>(provider);
        Services.Should().ContainTransient<IAuthorizationHandler>(provider);
        Services.Should().ContainSingleTransient<IAuthorizationService>(provider);
        Services.Should().ContainSingleTransient<IAuthorizationPolicyProvider>(provider);
        Services.Should().ContainSingleTransient<IAuthorizationHandlerProvider>(provider);
        Services.Should().ContainSingleTransient<IAuthorizationEvaluator>(provider);
        Services.Should().ContainSingleTransient<IAuthorizationHandlerContextFactory>(provider);
        Services.Should().ContainSingleTransient<IPolicyEvaluator>(provider);
        Services.Should().ContainSingleTransient<IAuthorizationMiddlewareResultHandler>(provider);

        var swaggerOptions = new SwaggerGenOptions();
        Services.Should().ContainConfigureOptions<SwaggerGenOptions>(provider)
            .Which.Configure(swaggerOptions);
        swaggerOptions.SwaggerGeneratorOptions.SwaggerDocs.Should().ContainKey("v1")
            .WhoseValue.Version.Should().Be("1");
    }

    [Fact]
    public void ShouldUse()
    {
        Services
            .AddDomain()
            .AddMediatR()
            .AddWeb(
            Web.AssemblyReference.Assembly,
            Web.UserManagement.AssemblyReference.Assembly,
            Contracts.Validations.AssemblyReference.Assembly,
            Contracts.UserManagement.Validators.AssemblyReference.Assembly);
        var appBuilder = new TestAppBuilder(Services);

        Action action = () => appBuilder.UseWeb();

        action.Should().NotThrow();
    }
}
