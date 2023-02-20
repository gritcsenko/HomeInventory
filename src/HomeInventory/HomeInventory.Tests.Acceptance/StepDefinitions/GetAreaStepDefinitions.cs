using HomeInventory.Contracts;
using HomeInventory.Tests.Acceptance.Drivers;
using HomeInventory.Tests.Acceptance.Support;
using Humanizer;

namespace HomeInventory.Tests.Acceptance.StepDefinitions;

[Binding]
public class GetAreaStepDefinitions
{
    private readonly ScenarioContext _context;
    private readonly IHomeInventoryAPIDriver _apiDriver;

    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef
    public GetAreaStepDefinitions(ScenarioContext context, IHomeInventoryAPIDriver apiDriver)
    {
        // For storing and retrieving scenario-specific data see https://go.specflow.org/doc-sharingdata
        // To use the multiline text or the table argument of the scenario,
        // additional string/Table parameters can be defined on the step definition
        // method.
        _context = context;
        _apiDriver = apiDriver;
    }

    [Given(@$"That today is {Patterns.DateOnly}")]
    public void GivenThatTodayIs(DateOnly todayDate)
    {
        _apiDriver.SetToday(todayDate);
    }

    [Given(@"Following environment")]
    public void GivenFollowingEnvironment(Table table)
    {
        var products = new List<AvailableProductData>(table.Rows.Count);
        foreach (var row in table.Rows)
        {
            var product = new AvailableProductData(
                storeName: row["Store"],
                productName: row["Product"],
                price: row["Price"].ParseDecimal(),
                date: row["Expiration"].ParseDate(),
                volume: row["UnitVolume"].ParseDecimal());
            products.Add(product);
        }
        _context.Set(products, "Products");
    }

    [Given(@"Following context")]
    public void GivenFollowingContext(Table table)
    {
        var name = table.Header.First();
        var names = table.Rows.Select(row => row[name]).ToArray();
        _context.Set(names, name.Pluralize());
    }

    [Given(@"Registered users")]
    public async Task GivenRegisteredUsers(Table table)
    {
        var users = new List<RegisterRequest>(table.Rows.Count);
        foreach (var row in table.Rows)
        {
            var request = new RegisterRequest(
                FirstName: row["FirstName"],
                LastName: row["LastName"],
                Email: row["Email"],
                Password: row["Password"]);
            _ = await _apiDriver.Authentication.RegisterAsync(request);
            users.Add(request);
        }
        _context.Set(users, "RegisteredUsers");
    }

    [Given(@$"User {Patterns.QuotedName}")]
    public async Task GivenUser(string email)
    {
        var response = await LoginUserAsync(email);
        _context.Set(response, "Login");
    }

    [When(@"User gets all available areas")]
    public async Task WhenUserGetsAllAvailableAreas()
    {
        var areas = await _apiDriver.Area.GetAllAsync();
        _context.Set(areas, "Areas");
    }

    [Then(@"List of areas should contain")]
    public void ThenListOfAreasShouldContain(Table table)
    {
        var areas = _context.Get<AreaResponse[]>("Areas");
        var name = table.Header.First();
        var names = table.Rows.Select(row => row[name]).ToArray();
        areas.Select(a => a.Name).Should().BeEquivalentTo(names);
    }

    private async Task<LoginResponse> LoginUserAsync(string email)
    {
        var registeredUsers = _context.Get<IReadOnlyCollection<RegisterRequest>>("RegisteredUsers");
        var password = registeredUsers.First(u => u.Email == email).Password;
        var request = new LoginRequest(email, password);
        var response = await _apiDriver.Authentication.LoginAsync(request);
        return response;
    }
}
