namespace HomeInventory.Tests.Acceptance.Drivers;

internal sealed class TestingConfiguration : ITestingConfiguration
{
    public string EnvironmentName { get; init; } = "Testing";
}
