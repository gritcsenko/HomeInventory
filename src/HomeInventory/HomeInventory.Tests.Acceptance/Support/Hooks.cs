using BoDi;
using HomeInventory.Tests.Acceptance.Drivers;

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
        objectContainer.RegisterInstanceAs<ITestingConfiguration>(new TestingConfiguration { EnvironmentName = "Testing" });
        objectContainer.RegisterTypeAs<HomeInventoryAPIDriver, IHomeInventoryAPIDriver>().InstancePerContext();
    }

    [AfterScenario(Order = 1)]
    public void Cleanup(IObjectContainer objectContainer)
    {
    }
}