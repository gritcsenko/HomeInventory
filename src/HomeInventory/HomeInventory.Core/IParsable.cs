namespace HomeInventory.Core;

public interface IParsable<TSelf>
    where TSelf : IParsable<TSelf>
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
    static abstract Option<TSelf> TryParse(string text);
}
