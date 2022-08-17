using FluentValidation;
using FluentValidation.TestHelper;
using HomeInventory.Contracts.Validations;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Validation;
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

        results.ShouldHaveValidationErrorFor(c => c.Password);
    }

    private class Container
    {
        public string? Password { get; set; }
    }

    private static InlineValidator<Container> CreateSut() => new()
    {
        v => v.RuleFor(x => x.Password).Password()
    };
}
