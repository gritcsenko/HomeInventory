namespace HomeInventory.Tests.Framework;

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
        ToIndexedVariable(variable);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Fix for CA2225: Provide a method named 'ToIndexedVariable' or 'FromVariable' as an alternate for operator op_Implicit")]
    public static IndexedVariable<T> ToIndexedVariable(Variable<T> variable, int index = 0) =>
        variable.CreateIndexed(index);

    public IVariable<TOther> OfType<TOther>() => new Variable<TOther>(Name);

    public IndexedVariable<T> WithIndex(int index = 0) => CreateIndexed(index);

    private IndexedVariable<T> CreateIndexed(int index) => new(Name, index);

    IIndexedVariable<T> IVariable<T>.WithIndex(int index) => WithIndex(index);
}
