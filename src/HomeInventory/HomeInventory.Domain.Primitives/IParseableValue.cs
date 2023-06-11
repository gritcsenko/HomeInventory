using DotNext;

namespace HomeInventory.Domain.Primitives;

public interface IParseableValue<TSelf>
    where TSelf : notnull, IParseableValue<TSelf>
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
    abstract static TSelf Parse(string text);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
    abstract static Optional<TSelf> TryParse(string text);
}
