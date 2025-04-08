using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.UserManagement.ValueObjects;

public sealed class Email(string value) : ValueObject<Email>(value)
{
    public string Value => GetComponent<string>(0);

    public override string ToString() => Value;
}