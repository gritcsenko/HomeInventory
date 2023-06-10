namespace HomeInventory.Tests.Framework;

public interface IVariable
{
    string Name { get; }

    IVariable<T> OfType<T>();
}

public interface IVariable<T> : IVariable
{
    IIndexedVariable<T> WithIndex(int index = 0);
}
