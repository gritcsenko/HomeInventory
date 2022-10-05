using BoDi;
using HomeInventory.Tests.Acceptance.Drivers;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Tests.Acceptance.Support;

[Binding]
internal class Hooks
{
    public Hooks()
    {
    }

    [BeforeScenario(Order = 1)]
    public void RegisterDependencies(IObjectContainer objectContainer)
    {
        objectContainer.RegisterInstanceAs<ITestingConfiguration>(new TestingConfiguration { EnvironmentName = Environments.Development });
        objectContainer.RegisterTypeAs<HomeInventoryAPIDriver, IHomeInventoryAPIDriver>().InstancePerContext();
    }

    [AfterScenario(Order = 1)]
    public void Cleanup(IObjectContainer objectContainer)
    {
    }
}