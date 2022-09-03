namespace HomeInventory.Tests.Acceptance.StepDefinitions;

[Binding]
public class AddJugOfLiquidStepDefinitions
{
    private readonly ScenarioContext _context;

    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef
    public AddJugOfLiquidStepDefinitions(ScenarioContext context)
    {
        // For storing and retrieving scenario-specific data see https://go.specflow.org/doc-sharingdata
        // To use the multiline text or the table argument of the scenario,
        // additional string/Table parameters can be defined on the step definition
        // method.
        _context = context;
    }

    [Given(@"That today is (\d{2}/\d{2}/\d{4}) and following environment")]
    public void GivenThatTodayIsAndFollowingEnvironment(DateOnly todayDate, Table table)
    {
        _context.Set(todayDate, "Today");
    }

    [Given(@"following context")]
    public void GivenFollowingContext(Table table)
    {
        throw new PendingStepException();
    }

    [Given(@"User bought a (\d+(\.\d+)?) gallon jug of ""([^""]*)"" at (\d{2}/\d{2}/\d{4}) in ""([^""]*)""")]
    public void GivenUserBoughtJugToday(decimal volume, string productName, DateOnly buyDate, string storeName)
    {
        _context.Set(volume, "Gallons");
        _context.Set(productName, "Product");
        _context.Set(buyDate, "BuyDate");
        _context.Set(storeName, "Store");
    }

    [When(@"User stores jug in to the ""([^""]*)"" storage area")]
    public void WhenUserStoresJugInToTheStorageArea(string storageAreaName)
    {
        _context.Set(storageAreaName, "Area");
    }

    [Then(@"The ""([^""]*)"" storage area should contain (\d+(\.\d+)?) gallon jug of ""([^""]*)"" that will expire at (\d{2}/\d{2}/\d{4})")]
    public void ThenTheStorageAreaShouldContainGallonJugOf(string storageAreaName, decimal volume, string productName, DateOnly expirationDate)
    {
        _context.Pending();
    }

    [Then(@"Accounting has transaction registered: User bought (\d+(\.\d+)?) gallon jug of ""([^""]*)"" at (\d{2}/\d{2}/\d{4}) in ""([^""]*)"" and payed \$(.*)")]
    public void ThenAccountingHasTransactionRegisteredUserBoughtGallonJugOfAtInAndPayed(decimal volume, string productName, DateOnly buyDate, string storeName, decimal price)
    {
        _context.Pending();
    }
}
