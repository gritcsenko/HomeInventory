using DotNext;

namespace HomeInventory.Domain.Primitives;

public interface IParseableValue<TSelf>
    where TSelf : notnull, IParseableValue<TSelf>
{
#pragma warning disable CA1000 // Do not declare static members on generic types
    abstract static TSelf Parse(string text);

    abstract static Optional<TSelf> TryParse(string text);
#pragma warning restore CA1000 // Do not declare static members on generic types
}
