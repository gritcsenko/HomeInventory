using HomeInventory.Application;
using HomeInventory.Tests.Helpers;
using HomeInventory.Web;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Swagger;

namespace HomeInventory.Tests.DependencyInjection;

[Trait("Category", "Unit")]
public class WebDependencyInjectionTests : BaseTest
{
    private readonly IServiceCollection _services = new TestingServiceCollection();

    [Fact]
    public void ShouldRegister()
    {
        _services.AddWeb();

        _services.Should().ContainSingleSingleton<HealthCheckService>();
        _services.Should().ContainSingleSingleton<ProblemDetailsFactory>();
        _services.Should().ContainSingleSingleton<TypeAdapterConfig>();
        _services.Should().ContainSingleScoped<IMapper>();
        _services.Should().ContainSingleSingleton<IMappingAssemblySource>();
        _services.Should().ContainSingleSingleton<IControllerFactory>();
        _services.Should().ContainSingleSingleton<IApiDescriptionGroupCollectionProvider>();
        _services.Should().ContainSingleTransient<ISwaggerProvider>();
    }
}
