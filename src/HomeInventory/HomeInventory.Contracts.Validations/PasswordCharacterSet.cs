namespace HomeInventory.Contracts.Validations;

internal class PasswordCharacterSet : IPasswordCharacterSet
{
    private readonly Func<char, bool> _condition;

    public PasswordCharacterSet(Func<char, bool> condition, string name) => (_condition, Name) = (condition, name);

    public string Name { get; }

    public bool ContainsAny(IEnumerable<char> characters) => characters.Any(_condition);
}
