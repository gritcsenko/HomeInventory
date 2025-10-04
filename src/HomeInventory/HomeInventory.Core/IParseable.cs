namespace HomeInventory.Core;

public interface IParseable<TSelf>
    where TSelf : IParseable<TSelf>
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
    static abstract TSelf Parse(string text);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
    static abstract Option<TSelf> TryParse(string text);
}
