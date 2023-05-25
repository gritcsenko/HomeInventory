using HomeInventory.Contracts;
using HomeInventory.Tests.Acceptance.Drivers;
using HomeInventory.Tests.Acceptance.Support;

namespace HomeInventory.Tests.Acceptance.StepDefinitions;

[Binding]
public class UserRegistrationStepDefinitions
{
    private static class Keys
    {
        public const string Email = nameof(Email);
        public const string Password = nameof(Password);
        public const string UserId = nameof(UserId);
    }

    private readonly ScenarioContext _context;
    private readonly IHomeInventoryAPIDriver _apiDriver;

    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef
    public UserRegistrationStepDefinitions(ScenarioContext context, IHomeInventoryAPIDriver apiDriver)
    {
        // For storing and retrieving scenario-specific data see https://go.specflow.org/doc-sharingdata
        // To use the multiline text or the table argument of the scenario,
        // additional string/Table parameters can be defined on the step definition
        // method.
        _context = context;
        _apiDriver = apiDriver;
    }

    [Given(@$"User e-mail {Patterns.QuotedName}")]
    public void GivenUserEmail(string email)
    {
        _context.Set(email, Keys.Email);
    }

    [Given(@$"User password {Patterns.QuotedName}")]
    public void GivenUserPassword(string password)
    {
        _context.Set(password, Keys.Password);
    }

    [When(@"User registers new account")]
    public async Task WhenUserRegistersNewAccount()
    {
        var request = new RegisterRequest(_context.Get<string>(Keys.Email), _context.Get<string>(Keys.Password));
        var response = await RegisterAsync(request);
        _context.Set(response.UserId, Keys.UserId);
    }

    [Then(@"User should get an ID as a confirmation of the successful registration")]
    public void ThenUserShouldGetAnIDAsAConfirmationOfTheSuccessfulRegistration()
    {
        var userId = _context.Get<Guid>(Keys.UserId);
        userId.Should().NotBeEmpty();
    }

    private async ValueTask<RegisterResponse> RegisterAsync(RegisterRequest request) =>
        await _apiDriver.UserManagement.RegisterAsync(request);
}
