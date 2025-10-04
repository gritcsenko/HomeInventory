using FluentValidation;
using FluentValidation.TestHelper;
using HomeInventory.Contracts.UserManagement.Validators;

namespace HomeInventory.Tests.Validation;

[UnitTest]
public class PasswordValidatorTests : BaseTest
{
    [Theory]
    [InlineData(null)]
    [InlineData("0oO.")]
    [InlineData("0oO!")]
    [InlineData("0oO$")]
    [InlineData("0oO%")]
    [InlineData("0oO^")]
    [InlineData("0oO&")]
    [InlineData("0oO*")]
    [InlineData("0oO(")]
    [InlineData("0oO-")]
    [InlineData("0oO`")]
    [InlineData("0oO'")]
    [InlineData("0oO~")]
    public void Should_PassValidation(string? password)
    {
        var container = new Container { Password = password };
        var sut = CreateSut();

        var results = sut.TestValidate(container);

        results.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("0oO")]
    [InlineData("0o.")]
    [InlineData("0O.")]
    [InlineData("oO.")]
    public void Should_NotPassValidation(string? password)
    {
        var container = new Container { Password = password };
        var sut = CreateSut();

        var results = sut.TestValidate(container);

        results.ShouldHaveValidationErrorFor(static c => c.Password);
    }

    private class Container
    {
        public string? Password { get; set; }
    }

#pragma warning disable IDE0028 // Simplify collection initialization
    private static InlineValidator<Container> CreateSut() =>
        new()
        { static v => v.RuleFor(static x => x.Password).Password(),
        };
#pragma warning restore IDE0028 // Simplify collection initialization
}
