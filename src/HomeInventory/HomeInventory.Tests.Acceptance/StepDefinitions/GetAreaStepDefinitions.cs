using HomeInventory.Contracts;
using HomeInventory.Tests.Acceptance.Drivers;
using HomeInventory.Tests.Acceptance.Support;
using Humanizer;

namespace HomeInventory.Tests.Acceptance.StepDefinitions;

[Binding]
public class GetAreaStepDefinitions
{
    private static class Keys
    {
        public const string Products = nameof(Products);
        public const string RegisteredUsers = nameof(RegisteredUsers);
        public const string Areas = nameof(Areas);
        public const string Login = nameof(Login);
    }

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
        var products = table.Rows
            .Select(CreateAvailableProduct);
        _context.SetAll(products, Keys.Products);
    }

    [Given(@"Following areas")]
    public void GivenFollowingAreas(Table table)
    {
        var names = table.Rows
            .Select(row => row[0]);
        var key = table.Header
            .First()
            .Pluralize();
        _context.SetAll(names, key);
    }

    [Given(@"Registered users")]
    public async Task GivenRegisteredUsers(Table table)
    {
        var requests = table.Rows
            .Select(CreateGegisterRequest)
            .DoAsync(RegisterAsync);
        await _context.SetAllAsync(requests, Keys.RegisteredUsers);
    }

    [Given(@$"User {Patterns.QuotedName}")]
    public async Task GivenUser(string email)
    {
        var response = await LoginUserAsync(email);
        _context.Set(response, Keys.Login);
    }

    [When(@"User gets all available areas")]
    public async Task WhenUserGetsAllAvailableAreas()
    {
        var areas = _apiDriver.Area
            .GetAllAsync();
        await _context.SetAllAsync(areas, Keys.Areas);
    }

    [Then(@"List of areas should contain")]
    public void ThenListOfAreasShouldContain(Table table)
    {
        var names = table.Rows.Select(row => row[0]);
        _context.GetAll<AreaResponse>(Keys.Areas)
            .Select(a => a.Name)
            .Should().BeEquivalentTo(names);
    }

    private static AvailableProductData CreateAvailableProduct(TableRow row) =>
        new(
            storeName: row["Store"],
            productName: row["Product"],
            price: row["Price"].ParseDecimal(),
            date: row["Expiration"].ParseDate(),
            volume: row["UnitVolume"].ParseDecimal());

    private static RegisterRequest CreateGegisterRequest(TableRow row) =>
        new(
            FirstName: row["FirstName"],
            LastName: row["LastName"],
            Email: row["Email"],
            Password: row["Password"]);

    private async ValueTask RegisterAsync(RegisterRequest request) =>
        _ = await _apiDriver.Authentication.RegisterAsync(request);

    private async ValueTask<LoginResponse> LoginUserAsync(string email)
    {
        var registeredUsers = _context.GetAll<RegisterRequest>(Keys.RegisteredUsers);
        var matchedUser = registeredUsers.First(u => u.Email == email);
        var password = matchedUser.Password;
        var request = new LoginRequest(email, password);
        var response = await _apiDriver.Authentication.LoginAsync(request);
        return response;
    }
}
