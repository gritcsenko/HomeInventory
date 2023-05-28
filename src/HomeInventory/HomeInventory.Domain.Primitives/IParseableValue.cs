using DotNext;

namespace HomeInventory.Domain.Primitives;

public interface IParseableValue<TSelf>
    where TSelf : notnull
{
    abstract static TSelf Parse(string text);

    abstract static Optional<TSelf> TryParse(string text);
}
