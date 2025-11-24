using HomeInventory.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.FeatureManagement;
using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Tests.Modules;

[UnitTest]
public sealed class ModuleServicesContextTests() : BaseTest<ModuleServicesContextTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void Constructor_ShouldPreserveServices()
    {
        Given
            .Services(out var servicesVar)
            .Sut(out var sutVar, servicesVar);

        var then = When
            .Invoked(sutVar, static sut => sut.Services);

        then
            .Result(servicesVar, static (result, expected) => result.Should().BeSameAs(expected));
    }

    [Fact]
    public void Constructor_ShouldPreserveConfiguration()
    {
        Given
            .Configuration(out var configurationVar)
            .Sut(out var sutVar, configurationVar);

        var then = When
            .Invoked(sutVar, static sut => sut.Configuration);

        then
            .Result(configurationVar, static (result, expected) => result.Should().BeSameAs(expected));
    }

    [Fact]
    public void Constructor_ShouldPreserveMetrics()
    {
        Given
            .Metrics(out var metricsVar)
            .Sut(out var sutVar, metricsVar);

        var then = When
            .Invoked(sutVar, static sut => sut.Metrics);

        then
            .Result(metricsVar, static (result, expected) => result.Should().BeSameAs(expected));
    }
}

public sealed class ModuleServicesContextTestsGivenContext(BaseTest test) : GivenContext<ModuleServicesContextTestsGivenContext>(test)
{
    public ModuleServicesContextTestsGivenContext Services(out IVariable<IServiceCollection> servicesVar) =>
        New(out servicesVar, static () => new ServiceCollection());

    public ModuleServicesContextTestsGivenContext Configuration(out IVariable<IConfiguration> configurationVar) =>
        SubstituteFor(out configurationVar);

    public ModuleServicesContextTestsGivenContext Metrics(out IVariable<IMetricsBuilder> metricsVar) =>
        SubstituteFor(out metricsVar);

    public ModuleServicesContextTestsGivenContext Sut(
        out IVariable<ModuleServicesContext> sutVar,
        IVariable<IServiceCollection> servicesVar) =>
        New(out sutVar, servicesVar, services =>
        {
            var config = Substitute.For<IConfiguration>();
            var metrics = Substitute.For<IMetricsBuilder>();
            var fm = Substitute.For<IFeatureManager>();
            var modules = Array.Empty<IModule>();
            return new(services, config, metrics, fm, modules);
        });

    public ModuleServicesContextTestsGivenContext Sut(
        out IVariable<ModuleServicesContext> sutVar,
        IVariable<IConfiguration> configurationVar) =>
        New(out sutVar, configurationVar, config =>
        {
            var services = new ServiceCollection();
            var metrics = Substitute.For<IMetricsBuilder>();
            var fm = Substitute.For<IFeatureManager>();
            var modules = Array.Empty<IModule>();
            return new(services, config, metrics, fm, modules);
        });

    public ModuleServicesContextTestsGivenContext Sut(
        out IVariable<ModuleServicesContext> sutVar,
        IVariable<IMetricsBuilder> metricsVar) =>
        New(out sutVar, metricsVar, metrics =>
        {
            var services = new ServiceCollection();
            var config = Substitute.For<IConfiguration>();
            var fm = Substitute.For<IFeatureManager>();
            var modules = Array.Empty<IModule>();
            return new(services, config, metrics, fm, modules);
        });
}

