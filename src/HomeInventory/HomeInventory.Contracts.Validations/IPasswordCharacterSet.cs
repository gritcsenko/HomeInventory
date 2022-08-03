namespace HomeInventory.Contracts.Validations;

internal interface IPasswordCharacterSet
{
    string Name { get; }

    bool ContainsAll(IEnumerable<char> characters);
}
