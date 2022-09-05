using HomeInventory.Contracts;
using HomeInventory.Tests.Acceptance.Drivers;
using HomeInventory.Tests.Acceptance.Support;
using Humanizer;

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
        _apiDriver.SetToday(todayDateText.ParseDate());
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
        var name = table.Header.First();
        _context.Set(table.Rows.Select(row => row[name]).ToArray(), name.Pluralize());
    }

    [Given(@"Registered users")]
    public async Task GivenRegisteredUsers(Table table)
    {
        var users = new Dictionary<Guid, RegisterRequest>(table.Rows.Count);
        foreach (var row in table.Rows)
        {
            var request = new RegisterRequest(
                FirstName: row["FirstName"],
                LastName: row["LastName"],
                Email: row["Email"],
                Password: row["Password"]);
            var response = await _apiDriver.Authentication.RegisterAsync(request);
            users.Add(response.Id, request);
        }
        _context.Set(users, "RegisteredUsers");
    }

    [Given(@$"User {Patterns.QuotedName} bought a {Patterns.CountWithDecimals} gallon jug of {Patterns.QuotedName} at {Patterns.DateOnly} in {Patterns.QuotedName}")]
    public async Task GivenUserBoughtJugToday(string email, decimal volume, string productName, string buyDateText, string storeName)
    {
        var response = await LoginUserAsync(email);

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

    private async Task<LoginResponse> LoginUserAsync(string email)
    {
        var registeredUsers = _context.Get<IReadOnlyDictionary<Guid, RegisterRequest>>("RegisteredUsers");
        var password = registeredUsers.Values.First(u => u.Email == email).Password;
        var request = new LoginRequest(email, password);
        var response = await _apiDriver.Authentication.LoginAsync(request);
        _context.Set(response, "Login");
        return response;
    }
}
