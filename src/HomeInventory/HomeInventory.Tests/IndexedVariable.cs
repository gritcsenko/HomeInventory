namespace HomeInventory.Tests;

public class IndexedVariable<T> : IIndexedVariable<T>
{
    public IndexedVariable(string name, int index)
    {
        Name = name;
        Index = index;
    }

    public string Name { get; }

    public int Index { get; }

    public IVariable<TOther> OfType<TOther>() => new Variable<TOther>(Name);

    public IIndexedVariable<T> WithIndex(int index = 0) => new IndexedVariable<T>(Name, index);
}
