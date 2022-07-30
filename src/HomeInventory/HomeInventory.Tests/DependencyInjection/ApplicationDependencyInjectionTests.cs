using FluentAssertions;
using HomeInventory.Application;
using HomeInventory.Tests.Helpers;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.DependencyInjection;

[Trait("Category", "Unit")]
public class ApplicationDependencyInjectionTests : BaseTest
{
    private readonly IServiceCollection _services = new TestingServiceCollection();

    [Fact]
    public void ShouldRegister()
    {
        _services.AddApplication();

        _services.Should().ContainSingleTransient<IMediator>();
        _services.Should().ContainSingleSingleton(typeof(IPipelineBehavior<,>));
        _services.Should().ContainSingleSingleton<IStartupFilter>();
        _services.Should().ContainSingleSingleton<IMappingAssemblySource>();
    }
}
