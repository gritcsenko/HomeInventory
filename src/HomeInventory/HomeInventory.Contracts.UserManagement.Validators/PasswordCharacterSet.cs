namespace HomeInventory.Contracts.UserManagement.Validators;

internal sealed class PasswordCharacterSet(Func<char, bool> condition, string name) : IPasswordCharacterSet
{
    private readonly Func<char, bool> _condition = condition;

    public string Name { get; } = name;

    public bool ContainsAny(IEnumerable<char> characters) => characters.Any(_condition);
}
