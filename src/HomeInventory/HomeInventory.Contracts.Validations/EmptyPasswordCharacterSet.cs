namespace HomeInventory.Contracts.Validations;

internal class EmptyPasswordCharacterSet : IPasswordCharacterSet
{
    public bool IsEmpty => true;

    public bool ContainsAll(IEnumerable<char> characters) => false;
}
