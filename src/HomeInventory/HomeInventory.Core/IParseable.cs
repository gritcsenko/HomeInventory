using DotNext;

namespace HomeInventory.Core;

public interface IParseable<TSelf>
    where TSelf : notnull, IParseable<TSelf>
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
    abstract static TSelf Parse(string text);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
    abstract static Optional<TSelf> TryParse(string text);
}
