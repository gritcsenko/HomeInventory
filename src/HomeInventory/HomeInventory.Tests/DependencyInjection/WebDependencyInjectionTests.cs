using FluentAssertions;
using HomeInventory.Application;
using HomeInventory.Tests.Helpers;
using HomeInventory.Web;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
        var env = Substitute.For<IWebHostEnvironment>();
        env.WebRootFileProvider.Returns(new NullFileProvider());
        _services.AddSingleton(env);
        _services.AddSingleton<IHostEnvironment>(env);
    }

    [Fact]
    public void ShouldRegister()
    {
        _services.AddWeb();
        var provider = _factory.CreateServiceProvider(_services);

        _services.Should().ContainSingleSingleton<HealthCheckService>(provider);
        _services.Should().ContainSingleSingleton<ProblemDetailsFactory>(provider);
        _services.Should().ContainSingleSingleton<TypeAdapterConfig>(provider);
        _services.Should().ContainSingleScoped<IMapper>(provider);
        _services.Should().ContainSingleSingleton<IMappingAssemblySource>(provider);
        _services.Should().ContainSingleSingleton<IControllerFactory>(provider);
        _services.Should().ContainSingleTransient<ISwaggerProvider>(provider);

        var swaggerOptions = new SwaggerGenOptions();
        _services.Should().ContainSingleSingleton<IConfigureOptions<SwaggerGenOptions>>(provider)
            .Which.Configure(swaggerOptions);
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
