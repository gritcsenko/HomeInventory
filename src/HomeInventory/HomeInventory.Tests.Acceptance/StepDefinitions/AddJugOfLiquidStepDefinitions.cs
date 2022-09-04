using HomeInventory.Tests.Acceptance.Drivers;
using HomeInventory.Tests.Acceptance.Support;

namespace HomeInventory.Tests.Acceptance.StepDefinitions;

[Binding]
public class AddJugOfLiquidStepDefinitions
{
    private readonly ScenarioContext _context;
    private readonly IHomeInventoryAPIDriver _apiDriver;

    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef
    public AddJugOfLiquidStepDefinitions(ScenarioContext context, IHomeInventoryAPIDriver apiDriver)
    {
        // For storing and retrieving scenario-specific data see https://go.specflow.org/doc-sharingdata
        // To use the multiline text or the table argument of the scenario,
        // additional string/Table parameters can be defined on the step definition
        // method.
        _context = context;
        _apiDriver = apiDriver;
    }

    [Given(@$"That today is {Patterns.DateOnly} and following environment")]
    public void GivenThatTodayIsAndFollowingEnvironment(string todayDateText, Table table)
    {
        _context.Set(todayDateText.ParseDate(), "Today");
        var products = new List<AvailableProductData>(table.Rows.Count);
        foreach (var row in table.Rows)
        {
            var product = new AvailableProductData(
                storeName: row["Store"],
                productName: row["Product"],
                priceText: row["Price"],
                dateText: row["Expiration"],
                volumeText: row["UnitVolume"]);
            products.Add(product);
        }
        _context.Set(products, "Products");
    }

    [Given(@$"Following context")]
    public void GivenFollowingContext(Table table)
    {
        _context.Set(table.Rows.Select(row => row["Area"]).ToArray(), "Areas");
    }

    [Given(@$"User bought a {Patterns.CountWithDecimals} gallon jug of {Patterns.QuotedName} at {Patterns.DateOnly} in {Patterns.QuotedName}")]
    public void GivenUserBoughtJugToday(decimal volume, string productName, string buyDateText, string storeName)
    {
        _context.Set(volume, "Gallons");
        _context.Set(productName, "Product");
        _context.Set(buyDateText.ParseDate(), "BuyDate");
        _context.Set(storeName, "Store");
    }

    [When(@$"User stores jug in to the {Patterns.QuotedName} storage area")]
    public void WhenUserStoresJugInToTheStorageArea(string storageAreaName)
    {
        _context.Set(storageAreaName, "Area");
    }

    [Then(@$"The {Patterns.QuotedName} storage area should contain {Patterns.CountWithDecimals} gallon jug of {Patterns.QuotedName} that will expire at {Patterns.DateOnly}")]
    public void ThenTheStorageAreaShouldContainGallonJugOf(string storageAreaName, decimal volume, string productName, string expirationDateText)
    {
        var expirationDate = expirationDateText.ParseDate();
        _context.Pending();
    }

    [Then(@$"A transaction was registered: User bought {Patterns.CountWithDecimals} gallon jug of {Patterns.QuotedName} at {Patterns.DateOnly} in {Patterns.QuotedName} and payed {Patterns.Price}")]
    public void ThenTransactionRegisteredWithUserBoughtGallonJugAndPayed(decimal volume, string productName, string buyDateText, string storeName, decimal price)
    {
        var buyDate = buyDateText.ParseDate();
        _context.Pending();
    }
}
