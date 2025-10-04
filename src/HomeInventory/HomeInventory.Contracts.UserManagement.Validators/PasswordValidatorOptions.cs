namespace HomeInventory.Contracts.UserManagement.Validators;

internal sealed class PasswordValidatorOptions
{
    public bool Numeric { get; init; } = true;

    public bool Uppercase { get; init; } = true;

    public bool Lowercase { get; init; } = true;

    public bool NonAlphanumeric { get; init; } = true;

    public IEnumerable<IPasswordCharacterSet> GetCharacterSets()
    {
        if (Numeric)
        {
            yield return PasswordCharacterSets.Numeric;
        }

        if (Uppercase)
        {
            yield return PasswordCharacterSets.Uppercase;
        }

        if (Lowercase)
        {
            yield return PasswordCharacterSets.Lowercase;
        }

        if (NonAlphanumeric)
        {
            yield return PasswordCharacterSets.NonAlphanumeric;
        }
    }
}
