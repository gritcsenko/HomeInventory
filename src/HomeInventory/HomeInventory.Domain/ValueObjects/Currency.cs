using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class Currency : ValueObject<Currency>
{
    public Currency(string name)
        : base(name)
    {
        Name = name;
    }

    public string Name { get; }
}
