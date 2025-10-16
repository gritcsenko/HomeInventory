using System.Diagnostics.CodeAnalysis;
using HomeInventory.Tests.Acceptance.Drivers;
using Reqnroll.BoDi;

namespace HomeInventory.Tests.Acceptance.Support;

[Binding]
public sealed class Hooks
{
    [BeforeScenario(Order = 1)]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public void RegisterDependencies(IObjectContainer objectContainer)
    {
        objectContainer.RegisterInstanceAs<ITestingConfiguration>(new TestingConfiguration { EnvironmentName = "Testing" });
        objectContainer.RegisterTypeAs<HomeInventoryApiDriver, IHomeInventoryApiDriver>().InstancePerContext();
    }

    [AfterScenario(Order = 1)]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public void Cleanup(IObjectContainer objectContainer)
    {
        // Nothing to clean up yet
    }
}
