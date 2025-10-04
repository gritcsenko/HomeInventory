using Range = System.Range;

namespace HomeInventory.Tests.Framework;

public static class VariablesCollectionExtensions
{
    public static PropertyValue<T> Get<T>(this VariablesContainer collection, IIndexedVariable<T> variable) =>
        collection.TryGet(variable)
            .ThrowIfNone(() => new InvalidOperationException($"Failed to get {variable.Name} of type {typeof(T)} at index {variable.Index}"));

    public static IEnumerable<T> GetMany<T>(this VariablesContainer collection, IVariable<T> variable) =>
        collection.GetMany(variable, ..);

    public static IEnumerable<T> GetMany<T>(this VariablesContainer collection, IVariable<T> variable, Range range) =>
        collection.GetAll(variable).ToArray()[range];
}
