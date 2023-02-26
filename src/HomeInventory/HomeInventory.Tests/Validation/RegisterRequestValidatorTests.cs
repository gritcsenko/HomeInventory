using FluentValidation.TestHelper;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Validations;

namespace HomeInventory.Tests.Validation;

[Trait("Category", "Unit")]
public class RegisterRequestValidatorTests : BaseTest
{
    [Theory]
    [MemberData(nameof(ValidCases))]
    public void Should_PassValidation(string? firstName, string? lastName, string? email, string? password)
    {
        var container = new RegisterRequest(firstName!, lastName!, email!, password!);
        var sut = CreateSut();

        var results = sut.TestValidate(container);

        results.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(InvalidCases))]
    public void Should_NotPassValidation(string? firstName, string? lastName, string? email, string? password)
    {
        var container = new RegisterRequest(firstName!, lastName!, email!, password!);
        var sut = CreateSut();

        var results = sut.TestValidate(container);

        results.ShouldHaveAnyValidationError();
    }

    private static RegisterRequestValidator CreateSut() => new();

    public static TheoryData<string?, string?, string?, string?> ValidCases() => new()
    {
        { "First", "Last", "anonymous.user@none.email", "123456789sS$" },
    };

    public static TheoryData<string?, string?, string?, string?> InvalidCases() => new()
    {
        { "", "Last", "anonymous.user@none.email", "123456789sS$" },
        { null, "Last", "anonymous.user@none.email", "123456789sS$" },
        { "First", "", "anonymous.user@none.email", "123456789sS$" },
        { "First", null, "anonymous.user@none.email", "123456789sS$" },
        { "First", "Last", "none.email", "123456789sS$" },
        { "First", "Last", "", "123456789sS$" },
        { "First", "Last", null, "123456789sS$" },
        { "First", "Last", "anonymous.user@none.email", null },
        { "First", "Last", "anonymous.user@none.email", "" },
        { "First", "Last", "anonymous.user@none.email", "1234sS$" },
        { "First", "Last", "anonymous.user@none.email", "123456789sS" },
        { "First", "Last", "anonymous.user@none.email", "123456789s$" },
        { "First", "Last", "anonymous.user@none.email", "123456789S$" },
        { "First", "Last", "anonymous.user@none.email", "sssssssssS$" },
    };
}
