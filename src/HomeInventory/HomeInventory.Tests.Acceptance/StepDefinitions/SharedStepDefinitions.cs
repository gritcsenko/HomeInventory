using HomeInventory.Tests.Acceptance.Drivers;
using HomeInventory.Tests.Acceptance.Support;

namespace HomeInventory.Tests.Acceptance.StepDefinitions;

[Binding]
internal sealed class SharedStepDefinitions(IHomeInventoryApiDriver apiDriver)
{
    private readonly IHomeInventoryApiDriver _apiDriver = apiDriver;

    [Given(@$"That today is {Patterns.DateOnly}")]
    public void GivenThatTodayIs(DateOnly todayDate) => _apiDriver.SetToday(todayDate);
}
