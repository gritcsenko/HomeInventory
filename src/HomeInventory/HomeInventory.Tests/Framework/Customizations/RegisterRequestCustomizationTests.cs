using HomeInventory.Contracts.UserManagement;
using HomeInventory.Contracts.UserManagement.Validators;

namespace HomeInventory.Tests.Framework.Customizations;

[UnitTest]
public class RegisterRequestCustomizationTests : BaseTest
{
    [Fact]
    public async Task Customize_Should_ProvideCorrectCustomization()
    {
        Fixture.Customize(new RegisterRequestCustomization());
        var request = Fixture.Create<RegisterRequest>();
        var validator = new RegisterRequestValidator();

        var validationResult = await validator.ValidateAsync(request);

        validationResult.Errors.Should().BeEmpty();
    }
}
