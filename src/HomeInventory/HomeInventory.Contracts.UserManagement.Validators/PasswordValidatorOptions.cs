namespace HomeInventory.Contracts.Validations;

internal class PasswordValidatorOptions
{
    public bool Numeric { get; set; } = true;

    public bool Uppercase { get; set; } = true;

    public bool Lowercase { get; set; } = true;

    public bool NonAlphanumeric { get; set; } = true;

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
