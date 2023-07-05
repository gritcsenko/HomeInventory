namespace HomeInventory.Tests.Framework;

public static class VariablesCollectionExtensions
{
    public static T Get<T>(this VariablesContainer collection, IIndexedVariable<T> variable)
        where T : notnull =>
        collection
            .TryGet(variable)
            .OrThrow(() => new InvalidOperationException($"Failed to get {variable.Name} of type {typeof(T)} at index {variable.Index}"));

    public static IEnumerable<T> GetMany<T>(this VariablesContainer collection, IVariable<T> variable)
        where T : notnull =>
        collection.GetMany(variable, ..);

    public static IEnumerable<T> GetMany<T>(this VariablesContainer collection, IVariable<T> variable, Range range)
        where T : notnull
    {
        var start = range.Start.Value;
        var end = range.End.IsFromEnd
            ? int.MaxValue - range.End.Value
            : range.End.Value;
        return Enumerable.Range(start, end - start)
            .Select(variable.WithIndex)
            .Select(collection.TryGet)
            .TakeWhile(x => x.HasValue)
            .Select(x => x.Value);
    }
}
