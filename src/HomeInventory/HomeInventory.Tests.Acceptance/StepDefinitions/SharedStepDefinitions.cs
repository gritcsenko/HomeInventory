using HomeInventory.Tests.Acceptance.Drivers;

namespace HomeInventory.Tests.Acceptance.StepDefinitions;

[Binding]
internal sealed class SharedStepDefinitions(IHomeInventoryApiDriver apiDriver)
{
    private readonly IHomeInventoryApiDriver _apiDriver = apiDriver;

    [Given("That today is {DateOnly}")]
    public void GivenThatTodayIs(DateOnly todayDate) => _apiDriver.SetToday(todayDate);
}
