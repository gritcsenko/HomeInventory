namespace HomeInventory.Contracts.Validations;

internal interface IPasswordCharacterSet
{
    bool IsEmpty { get; }

    bool ContainsAll(IEnumerable<char> characters);
}
