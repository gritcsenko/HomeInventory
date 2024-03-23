using System.Globalization;
using HomeInventory.Contracts;
using HomeInventory.Core;
using HomeInventory.Tests.Acceptance.Drivers;
using HomeInventory.Tests.Acceptance.Support;
using Humanizer;

namespace HomeInventory.Tests.Acceptance.StepDefinitions;

[Binding]
internal class GetAreaStepDefinitions(ScenarioContext context, IHomeInventoryApiDriver apiDriver)
{
    private static class Keys
    {
        public const string Products = nameof(Products);
        public const string RegisteredUsers = nameof(RegisteredUsers);
        public const string Areas = nameof(Areas);
        public const string Login = nameof(Login);
    }

    private readonly ScenarioContext _context = context;
    private readonly IHomeInventoryApiDriver _apiDriver = apiDriver;
    private readonly CultureInfo _culture = CultureInfo.CurrentCulture;

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
        var requests = table.Rows.Select(CreateGegisterRequest).ToReadOnly();
        foreach (var request in requests)
        {
            await RegisterAsync(request);
        }
        _context.SetAll(requests, Keys.RegisteredUsers);
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

    private AvailableProductData CreateAvailableProduct(TableRow row) =>
        new(
            storeName: row["Store"],
            productName: row["Product"],
            price: row["Price"].ParseDecimal(_culture.NumberFormat),
            date: row["Expiration"].ParseDate(formatProvider: _culture.DateTimeFormat),
            volume: row["UnitVolume"].ParseDecimal(_culture.NumberFormat));

    private static RegisterRequest CreateGegisterRequest(TableRow row) =>
        new(
            Email: row["Email"],
            Password: row["Password"]);

    private async ValueTask RegisterAsync(RegisterRequest request) =>
        _ = await _apiDriver.UserManagement.RegisterAsync(request);

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
