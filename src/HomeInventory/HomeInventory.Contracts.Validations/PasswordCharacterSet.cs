namespace HomeInventory.Contracts.Validations;

internal class PasswordCharacterSet : IPasswordCharacterSet
{
    private readonly HashSet<char> _set;

    public PasswordCharacterSet(IEnumerable<char> collection) => _set = new(collection);

    public bool IsEmpty => _set.Count == 0;

    public bool ContainsAll(IEnumerable<char> characters) => characters.All(_set.Contains);
}
