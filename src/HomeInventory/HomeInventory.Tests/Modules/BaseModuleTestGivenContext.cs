using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;

namespace HomeInventory.Tests.Modules;

public abstract class BaseModuleTestGivenContext<TGiven, TModule>(BaseTest test) : GivenContext<TGiven, TModule>(test)
    where TGiven : BaseModuleTestGivenContext<TGiven, TModule>
    where TModule : IModule
{
    public TGiven Services(out IVariable<IServiceCollection> services, int count = 1, [CallerArgumentExpression(nameof(services))] string? name = null) =>
        New(out services, CreateServiceCollection, count, name);

    public TGiven Configuration(out IVariable<IConfiguration> configuration, int count = 1, [CallerArgumentExpression(nameof(configuration))] string? name = null) =>
        New(out configuration, CreateConfiguration, count, name);

    protected virtual IServiceCollection CreateServiceCollection() => new ServiceCollection();

    protected virtual IConfiguration CreateConfiguration() => new ConfigurationBuilder().Build();
}
