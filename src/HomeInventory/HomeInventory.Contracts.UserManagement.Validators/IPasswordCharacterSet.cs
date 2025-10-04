namespace HomeInventory.Contracts.UserManagement.Validators;

internal interface IPasswordCharacterSet
{
    string Name { get; }

    bool ContainsAny(IEnumerable<char> characters);
}
