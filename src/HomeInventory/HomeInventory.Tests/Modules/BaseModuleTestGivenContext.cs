using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using System.Runtime.CompilerServices;

namespace HomeInventory.Tests.Modules;

public static class BaseModuleTestGivenContext
{
    public static FunctionalModuleTestGivenContext<TModule> Create<TModule>(BaseTest<FunctionalModuleTestGivenContext<TModule>> test, Func<TModule> createModuleFunc)
        where TModule : IModule =>
        new(test, createModuleFunc);
}

public abstract class BaseModuleTestGivenContext<TGiven, TModule>(BaseTest<TGiven> test) : GivenContext<TGiven, TModule>(test), IModuleTestGivenContext<TGiven>
    where TGiven : BaseModuleTestGivenContext<TGiven, TModule>, IModuleTestGivenContext<TGiven>
    where TModule : IModule
{
    public TGiven Services(out IVariable<IServiceCollection> services, int count = 1, [CallerArgumentExpression(nameof(services))] string? name = null) =>
        New(out services, CreateServiceCollection, count, name);

    public TGiven Configuration(out IVariable<IConfiguration> configuration, int count = 1, [CallerArgumentExpression(nameof(configuration))] string? name = null) =>
        New(out configuration, CreateConfiguration, count, name);

    public TGiven FeatureManager(out IVariable<IFeatureManager> manager, int count = 1, [CallerArgumentExpression(nameof(manager))] string? name = null) =>
        New(out manager, CreateFeatureManager, count, name);

    protected virtual IServiceCollection CreateServiceCollection() => new ServiceCollection();

    protected virtual IConfiguration CreateConfiguration() => new ConfigurationBuilder().Build();

    protected virtual IFeatureManager CreateFeatureManager() => Substitute.For<IFeatureManager>();
}
