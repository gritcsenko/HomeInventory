namespace HomeInventory.Tests;

public interface IIndexedVariable<T> : IVariable<T>
{
    int Index { get; }
}
