using FluentValidation.TestHelper;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Validations;

namespace HomeInventory.Tests.Validation;

[UnitTest]
public class LoginRequestValidatorTests : BaseTest
{
    [Theory]
    [MemberData(nameof(ValidCases))]
    public void Should_PassValidation(string? email, string? password)
    {
        var container = new LoginRequest(email!, password!);
        var sut = CreateSut();

        var results = sut.TestValidate(container);

        results.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(InvalidCases))]
    public void Should_NotPassValidation(string? email, string? password)
    {
        var container = new LoginRequest(email!, password!);
        var sut = CreateSut();

        var results = sut.TestValidate(container);

        results.ShouldHaveAnyValidationError();
    }

    private static LoginRequestValidator CreateSut() => new();

    public static TheoryData<string?, string?> ValidCases() => new()
    {
        { "anonymous.user@none.email", "123456789sS$" },
    };

    public static TheoryData<string?, string?> InvalidCases() => new()
    {
        { "none.email", "123456789sS$" },
        { "", "123456789sS$" },
        { null, "123456789sS$" },
        { "anonymous.user@none.email", null },
        { "anonymous.user@none.email", "" },
    };
}
