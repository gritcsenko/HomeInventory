using HomeInventory.Tests.Acceptance.Drivers;

namespace HomeInventory.Tests.Acceptance.Support;

[Binding]
public sealed class ScenarioHooks(ScenarioContext scenarioContext)
{
    private readonly ScenarioContext _scenarioContext = scenarioContext;

    [BeforeScenario(Order = 1)]
    public void RegisterDependencies()
    {
        var container = _scenarioContext.ScenarioContainer;
        var configuration = new TestingConfiguration
        {
            EnvironmentName = "Testing",
        };
        container.RegisterInstanceAs<ITestingConfiguration>(configuration);
        container.RegisterTypeAs<HomeInventoryApiDriver, IHomeInventoryApiDriver>().InstancePerContext();
    }

    [AfterScenario(Order = 1)]
    public static void Cleanup()
    {
        // Nothing to clean up yet
    }
}
