using FluentValidation.TestHelper;
using HomeInventory.Contracts.UserManagement;
using HomeInventory.Contracts.UserManagement.Validators;

namespace HomeInventory.Tests.Validation;

[UnitTest]
public class RegisterRequestValidatorTests : BaseTest
{
    [Theory]
    [MemberData(nameof(ValidCases))]
    public void Should_PassValidation(string? email, string? password)
    {
        var container = new RegisterRequest(email!, password!);
        var sut = CreateSut();

        var results = sut.TestValidate(container);

        results.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(InvalidCases))]
    public void Should_NotPassValidation(string? email, string? password)
    {
        var container = new RegisterRequest(email!, password!);
        var sut = CreateSut();

        var results = sut.TestValidate(container);

        results.ShouldHaveValidationErrors();
    }

    private static RegisterRequestValidator CreateSut() => new();

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
        { "anonymous.user@none.email", "1234sS$" },
        { "anonymous.user@none.email", "123456789sS" },
        { "anonymous.user@none.email", "123456789s$" },
        { "anonymous.user@none.email", "123456789S$" },
        { "anonymous.user@none.email", "sssssssssS$" },
    };
}
