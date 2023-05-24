using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class Email : ValueObject<Email>
{
    public Email(string value)
        : base(value)
    {
        Value = value;
    }

    public string Value { get; }
}
