namespace HomeInventory.Tests.Framework;

public class Variable(string name) : IVariable
{
    public string Name { get; } = name;

    public IVariable<T> OfType<T>() => new Variable<T>(Name);
}

public class Variable<T>(string name) : Variable(name), IVariable<T>
{
    public IIndexedVariable<T> this[int index] => CreateIndexed(index);

    public IIndexedVariable<T> WithIndex(int index = 0) => CreateIndexed(index);

    private IndexedVariable<T> CreateIndexed(int index) => new(Name, index);
}
