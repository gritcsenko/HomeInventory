using HomeInventory.Contracts.UserManagement;
using HomeInventory.Tests.Acceptance.Drivers;
using HomeInventory.Tests.Acceptance.Support;

namespace HomeInventory.Tests.Acceptance.StepDefinitions;

[Binding]
internal sealed class UserRegistrationStepDefinitions(ScenarioContext context, IHomeInventoryApiDriver apiDriver)
{
    private static class Keys
    {
        public const string Email = nameof(Email);
        public const string Password = nameof(Password);
        public const string UserId = nameof(UserId);
    }

    private readonly ScenarioContext _context = context;
    private readonly IHomeInventoryApiDriver _apiDriver = apiDriver;

    [Given(@$"User e-mail {Patterns.QuotedName}")]
    public void GivenUserEmail(string email) => _context.Set(email, Keys.Email);

    [Given(@$"User password {Patterns.QuotedName}")]
    public void GivenUserPassword(string password) => _context.Set(password, Keys.Password);

    [When(@"User registers new account")]
    public async Task WhenUserRegistersNewAccount()
    {
        var request = new RegisterRequest(_context.Get<string>(Keys.Email), _context.Get<string>(Keys.Password));
        var response = await RegisterAsync(request);
        _context.Set(response.UserId, Keys.UserId);
    }

    [Then(@"User should get an ID as a confirmation of the successful registration")]
    public void ThenUserShouldGetAnId()
    {
        var userId = _context.Get<string>(Keys.UserId);
        userId.Should().NotBeEmpty();
    }

    private async ValueTask<RegisterResponse> RegisterAsync(RegisterRequest request) =>
        await _apiDriver.UserManagement.RegisterAsync(request);
}
