using HomeInventory.Tests.Acceptance.Drivers;
using Reqnroll.BoDi;

namespace HomeInventory.Tests.Acceptance.Support;

[Binding]
public sealed class Hooks
{
    [BeforeScenario(Order = 1)]
#pragma warning disable CA1822 // Mark members as static
    public void RegisterDependencies(IObjectContainer objectContainer)
#pragma warning restore CA1822 // Mark members as static
    {
        objectContainer.RegisterInstanceAs<ITestingConfiguration>(new TestingConfiguration { EnvironmentName = "Testing" });
        objectContainer.RegisterTypeAs<HomeInventoryApiDriver, IHomeInventoryApiDriver>().InstancePerContext();
    }

    [AfterScenario(Order = 1)]
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
    public void Cleanup(IObjectContainer objectContainer)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
    {
        // Nothing to clean up yet
    }
}
