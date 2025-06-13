using FluentValidation.TestHelper;
using HomeInventory.Web.Configuration;
using HomeInventory.Web.Configuration.Validation;

namespace HomeInventory.Tests.Validation;

[UnitTest]
public sealed class JwtOptionsValidatorTests : BaseTest
{
    [Theory]
    [MemberData(nameof(ValidCases))]
    public void Should_PassValidation(string? secret, string? issuer, string? audience, string? algorithm, TimeSpan expiry)
    {
        var objectToTest = CreateObjectToTest(secret!, issuer!, audience!, algorithm!, expiry);
        var sut = CreateSut();

        var results = sut.TestValidate(objectToTest);

        results.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(InvalidCases))]
    public void Should_NotPassValidation(string? secret, string? issuer, string? audience, string? algorithm, TimeSpan expiry)
    {
        var objectToTest = CreateObjectToTest(secret!, issuer!, audience!, algorithm!, expiry);
        var sut = CreateSut();

        var results = sut.TestValidate(objectToTest);

        results.ShouldHaveValidationErrors();
    }

    private static JwtOptionsValidator CreateSut() => new();

    private static JwtOptions CreateObjectToTest(string secret, string issuer, string audience, string algorithm, TimeSpan expiry) =>
        new()
        {
            Secret = secret,
            Issuer = issuer,
            Audience = audience,
            Algorithm = algorithm,
            Expiry = expiry,
        };

    public static TheoryData<string?, string?, string?, string?, TimeSpan> ValidCases() => new()
    {
        { "Secret!", "HomeInventory", "HomeInventory", "HS256", TimeSpan.FromMinutes(1) },
    };

    public static TheoryData<string?, string?, string?, string?, TimeSpan> InvalidCases() => new()
    {
        { "", "HomeInventory", "HomeInventory", "HS256", TimeSpan.FromMinutes(1) },
        { "Secret!", "", "HomeInventory", "HS256", TimeSpan.FromMinutes(1) },
        { "Secret!", "HomeInventory", "", "HS256", TimeSpan.FromMinutes(1) },
        { "Secret!", "HomeInventory", "HomeInventory", "", TimeSpan.FromMinutes(1) },
        { "Secret!", "HomeInventory", "HomeInventory", "HS256", TimeSpan.FromSeconds(1) },
    };
}
