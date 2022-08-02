namespace HomeInventory.Contracts.Validations;

internal static class PasswordCharacterSets
{
    public static IPasswordCharacterSet Empty { get; } = new EmptyPasswordCharacterSet();

    public static IPasswordCharacterSet Numeric { get; } = new ConditionalPasswordCharacterSet(char.IsDigit);

    public static IPasswordCharacterSet Uppercase { get; } = new ConditionalPasswordCharacterSet(char.IsUpper);

    public static IPasswordCharacterSet Lowercase { get; } = new ConditionalPasswordCharacterSet(char.IsLower);

    public static IPasswordCharacterSet NonAlphanumeric { get; } = new ConditionalPasswordCharacterSet(char.IsSymbol);

    public static IPasswordCharacterSet UnionWith(this IPasswordCharacterSet set, IPasswordCharacterSet other)
    {
        if (set.IsEmpty)
        {
            return other;
        }

        if (other.IsEmpty)
        {
            return set;
        }

        return new CompositePasswordCharacterSet(set, other);
    }
}
