namespace HomeInventory.Contracts.Validations;

internal class PasswordValidatorOptions
{
    public bool Numeric { get; set; } = true;

    public bool Uppercase { get; set; } = true;

    public bool Lowercase { get; set; } = true;

    public bool NonAlphanumeric { get; set; } = true;

    public IPasswordCharacterSet GetCharacterSet()
    {
        var result = PasswordCharacterSets.Empty;
        if (Numeric)
        {
            result = result.UnionWith(PasswordCharacterSets.Numeric);
        }
        if (Uppercase)
        {
            result = result.UnionWith(PasswordCharacterSets.Uppercase);
        }
        if (Lowercase)
        {
            result = result.UnionWith(PasswordCharacterSets.Lowercase);
        }
        if (NonAlphanumeric)
        {
            result = result.UnionWith(PasswordCharacterSets.NonAlphanumeric);
        }
        return result;
    }
}
