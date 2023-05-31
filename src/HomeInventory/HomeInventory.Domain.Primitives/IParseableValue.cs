using DotNext;

namespace HomeInventory.Domain.Primitives;

public interface IParseableValue<TSelf>
    where TSelf : notnull, IParseableValue<TSelf>
{
    abstract static TSelf Parse(string text);

    abstract static Optional<TSelf> TryParse(string text);
}
