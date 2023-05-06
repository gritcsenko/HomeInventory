using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests;

public static class VariablesCollectionExtensions
{
    public static T Get<T>(this VariablesCollection collection, IIndexedVariable<T> variable)
        where T : notnull =>
        collection
            .TryGet(variable)
            .Reduce(() => throw new InvalidOperationException($"Failed to get {variable.Name} of type {typeof(T)} at index {variable.Index}"));
}
