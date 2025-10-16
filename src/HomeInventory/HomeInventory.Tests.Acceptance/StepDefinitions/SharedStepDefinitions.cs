using System.Globalization;
using HomeInventory.Tests.Acceptance.Drivers;
using HomeInventory.Tests.Acceptance.Support;

namespace HomeInventory.Tests.Acceptance.StepDefinitions;

[Binding]
internal sealed class SharedStepDefinitions(IHomeInventoryApiDriver apiDriver)
{
    private readonly IHomeInventoryApiDriver _apiDriver = apiDriver;

    [StepArgumentTransformation(Patterns.DateOnly, Name = nameof(Patterns.DateOnly))]
    public static DateOnly ParseDateOnly(string s) =>
        DateOnly.ParseExact(s, "MM/dd/yyyy", CultureInfo.InvariantCulture);

    [Given("That today is {DateOnly}")]
    public void GivenThatTodayIs(DateOnly todayDate) => _apiDriver.SetToday(todayDate);
}
