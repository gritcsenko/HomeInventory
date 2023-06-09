﻿using BoDi;
using HomeInventory.Tests.Acceptance.Drivers;

namespace HomeInventory.Tests.Acceptance.Support;

[Binding]
public sealed class Hooks
{
    [BeforeScenario(Order = 1)]
#pragma warning disable CA1822 // Mark members as static
    public void RegisterDependencies(IObjectContainer objectContainer)
#pragma warning restore CA1822 // Mark members as static
    {
        if (objectContainer is null)
        {
            throw new ArgumentNullException(nameof(objectContainer));
        }

        objectContainer.RegisterInstanceAs<ITestingConfiguration>(new TestingConfiguration { EnvironmentName = "Testing" });
        objectContainer.RegisterTypeAs<HomeInventoryAPIDriver, IHomeInventoryAPIDriver>().InstancePerContext();
    }

    [AfterScenario(Order = 1)]
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
    public void Cleanup(IObjectContainer objectContainer)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
    {
    }
}
