namespace HomeInventory.Tests.Framework;

public interface IIndexedVariable<T> : IVariable<T>
{
    int Index { get; }
}
