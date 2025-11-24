using HomeInventory.Modules;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.FeatureManagement;

namespace HomeInventory.Tests.Modules;

[UnitTest]
public sealed class ModulesHostTests() : BaseTest<ModulesHostTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void Create_ShouldReturnModulesHost()
    {
        Given
            .EmptyModules(out var modulesVar);

        var then = When
            .Invoked(modulesVar, static modules => ModulesHost.Create(modules));

        then
            .Result(static result => result.Should().BeOfType<ModulesHost>());
    }

    [Fact]
    public async Task AddServicesAsync_WithNoModules_ReturnsRegisteredModules()
    {
        Given
            .EmptyModules(out var modulesVar)
            .Sut(out var sutVar, modulesVar)
            .Services(out var servicesVar)
            .Configuration(out var configurationVar)
            .Metrics(out var metricsVar);

        var then = await When
            .InvokedAsync(sutVar, servicesVar, configurationVar, metricsVar,
                static (sut, services, config, metrics, ct) =>
                    sut.AddServicesAsync(services, config, metrics, ct));

        then
            .Result(static result => result.Should().BeAssignableTo<IRegisteredModules>());
    }

    [Fact]
    public async Task AddServicesAsync_WithModules_RegistersFeatureManagement()
    {
        Given
            .ModulesWithSubjectModule(out var modulesVar)
            .Sut(out var sutVar, modulesVar)
            .Services(out var servicesVar)
            .Configuration(out var configurationVar)
            .Metrics(out var metricsVar);

        var then = await When
            .InvokedAsync(sutVar, servicesVar, configurationVar, metricsVar,
                static (sut, services, config, metrics, ct) =>
                    sut.AddServicesAsync(services, config, metrics, ct));

        then
            .Result(servicesVar, static (result, services) =>
            {
                result.Should().BeAssignableTo<IRegisteredModules>();
                services.Should().Contain(d => d.ServiceType == typeof(IFeatureManager));
            });
    }
}

public sealed class ModulesHostTestsGivenContext(BaseTest test) : GivenContext<ModulesHostTestsGivenContext>(test)
{
    public ModulesHostTestsGivenContext EmptyModules(out IVariable<IReadOnlyCollection<IModule>> modulesVar) =>
        New(out modulesVar, static () => []);

    public ModulesHostTestsGivenContext ModulesWithSubjectModule(out IVariable<IReadOnlyCollection<IModule>> modulesVar) =>
        New(out modulesVar, static () => [new SubjectModule()]);

    public ModulesHostTestsGivenContext Sut(
        out IVariable<ModulesHost> sutVar,
        IVariable<IReadOnlyCollection<IModule>> modulesVar) =>
        New(out sutVar, modulesVar, static modules => new(modules));

    public ModulesHostTestsGivenContext Services(out IVariable<IServiceCollection> servicesVar) =>
        New(out servicesVar, static () => new ServiceCollection());

    public ModulesHostTestsGivenContext Configuration(out IVariable<IConfiguration> configurationVar) =>
        SubstituteFor(out configurationVar, config => config.GetSection(Arg.Any<string>()).Returns(callInfo =>
        {
            var expectedPath = callInfo.Arg<string>();
            var section = Substitute.For<IConfigurationSection>();
            section.Key.Returns(expectedPath);
            section.Value.Returns((string?)null);
            section.Path.Returns(expectedPath);
            return section;
        }));

    public ModulesHostTestsGivenContext Metrics(out IVariable<IMetricsBuilder> metricsVar) =>
        SubstituteFor(out metricsVar);
}

