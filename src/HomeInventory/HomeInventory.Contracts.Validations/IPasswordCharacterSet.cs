namespace HomeInventory.Contracts.Validations;

internal interface IPasswordCharacterSet
{
    string Name { get; }

    bool ContainsAny(IEnumerable<char> characters);
}
