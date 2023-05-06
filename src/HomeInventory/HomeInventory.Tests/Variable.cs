namespace HomeInventory.Tests;

public class Variable : IVariable
{
    public string Name { get; }

    public Variable(string name)
    {
        Name = name;
    }

    public IVariable<T> OfType<T>() => new Variable<T>(Name);
}

public class Variable<T> : IVariable<T>
{
    public Variable(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public static implicit operator IndexedVariable<T>(Variable<T> variable) =>
        variable.CreateIndexed(0);

    public IVariable<TOther> OfType<TOther>() => new Variable<TOther>(Name);

    public IndexedVariable<T> WithIndex(int index = 0) => CreateIndexed(index);

    private IndexedVariable<T> CreateIndexed(int index) => new(Name, index);

    IIndexedVariable<T> IVariable<T>.WithIndex(int index) => WithIndex(index);
}
