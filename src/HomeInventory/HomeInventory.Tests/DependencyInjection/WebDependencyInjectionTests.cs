﻿using FluentAssertions;
using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Primitives;
using HomeInventory.Tests.Helpers;
using HomeInventory.Tests.Support;
using HomeInventory.Web;
using HomeInventory.Web.Authentication;
using HomeInventory.Web.Configuration;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using NSubstitute;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

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
        _services.Should().ContainSingleTransient<IPostConfigureOptions<JwtBearerOptions>>(provider);
        _services.Should().ContainSingleSingleton<IJwtIdentityGenerator>(provider);
        _services.Should().ContainSingleSingleton<IAuthenticationTokenGenerator>(provider);
        _services.Should().ContainSingleSingleton<HealthCheckService>(provider);
        _services.Should().ContainSingleSingleton<ProblemDetailsFactory>(provider);
        _services.Should().ContainSingleSingleton<TypeAdapterConfig>(provider);
        _services.Should().ContainSingleScoped<IMapper>(provider);
        _services.Should().ContainSingleSingleton<IMappingAssemblySource>(provider);
        _services.Should().ContainSingleSingleton<IControllerFactory>(provider);
        _services.Should().ContainSingleTransient<ISwaggerProvider>(provider);

        var swaggerOptions = new SwaggerGenOptions();
        _services.Should().ContainSingleTransient<IPostConfigureOptions<SwaggerGenOptions>>(provider)
            .Which.PostConfigure(string.Empty, swaggerOptions);
        swaggerOptions.SwaggerGeneratorOptions.SwaggerDocs.Should().ContainKey("v1")
            .WhoseValue.Version.Should().Be("1.0");
    }

    [Fact]
    public void ShouldUse()
    {
        _services.AddWeb();
        var appBuilder = new TestAppBuilder(_services, _factory);

        appBuilder.UseWeb();
    }
}
