using HomeInventory.Application;
using HomeInventory.Tests.Helpers;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.DependencyInjection;

[Trait("Category", "Unit")]
public class ApplicationDependencyInjectionTests : BaseTest
{
    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly IServiceProviderFactory<IServiceCollection> _factory = new DefaultServiceProviderFactory();

    [Fact]
    public void ShouldRegister()
    {
        _services.AddApplication();
        var provider = _factory.CreateServiceProvider(_services);

        _services.Should().ContainSingleSingleton(typeof(IPipelineBehavior<,>));
        _services.Should().ContainSingleSingleton<IStartupFilter>(provider);
        _services.Should().ContainSingleSingleton<IMappingAssemblySource>(provider);
    }
}
