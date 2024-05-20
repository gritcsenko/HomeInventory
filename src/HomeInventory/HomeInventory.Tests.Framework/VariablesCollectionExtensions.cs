namespace HomeInventory.Tests.Framework;

public static class VariablesCollectionExtensions
{
    public static T Get<T>(this VariablesContainer collection, IIndexedVariable<T> variable)
        where T : notnull
    {
        var result = collection.TryGet(variable);
        if (result.IsUndefined)
        {
           throw new InvalidOperationException($"Failed to get {variable.Name} of type {typeof(T)} at index {variable.Index}");
        }

        return result.ValueOrDefault!;
    }

    public static IEnumerable<T> GetMany<T>(this VariablesContainer collection, IVariable<T> variable)
        where T : notnull =>
        collection.GetMany(variable, ..);

    public static IEnumerable<T> GetMany<T>(this VariablesContainer collection, IVariable<T> variable, Range range)
        where T : notnull
    {
        var all = collection.GetAll(variable).ToArray();
        return all[range];
    }
}
