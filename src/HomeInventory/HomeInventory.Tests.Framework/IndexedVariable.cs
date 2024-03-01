namespace HomeInventory.Tests.Framework;

public sealed class IndexedVariable<T>(string name, int index) : Variable<T>(name), IIndexedVariable<T>
{
    public int Index { get; } = index;
}
