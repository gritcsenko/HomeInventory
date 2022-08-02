namespace HomeInventory.Contracts.Validations;

internal class ConditionalPasswordCharacterSet : IPasswordCharacterSet
{
    private readonly Func<char, bool> _condition;

    public ConditionalPasswordCharacterSet(Func<char, bool> condition) => _condition = condition;

    public bool IsEmpty => false;

    public bool ContainsAll(IEnumerable<char> characters) => characters.All(_condition);
}
