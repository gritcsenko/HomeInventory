namespace HomeInventory.Contracts.Validations;

internal static class PasswordCharacterSets
{
    public static IPasswordCharacterSet Numeric { get; } = new PasswordCharacterSet(char.IsDigit, "numeric");

    public static IPasswordCharacterSet Uppercase { get; } = new PasswordCharacterSet(char.IsUpper, "uppercase");

    public static IPasswordCharacterSet Lowercase { get; } = new PasswordCharacterSet(char.IsLower, "lowercase");

    public static IPasswordCharacterSet NonAlphanumeric { get; } = new PasswordCharacterSet(char.IsSymbol, "non-alphanumeric");
}
