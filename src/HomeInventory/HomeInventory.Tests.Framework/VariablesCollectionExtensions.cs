namespace HomeInventory.Tests.Framework;

public static class VariablesCollectionExtensions
{
    public static IEnumerable<T> Get<T>(this VariablesContainer collection, IVariable<T> variable, int count)
        where T : notnull =>
        Enumerable.Range(0, count)
            .Select(index => collection.Get(variable.WithIndex(index)));

    public static T Get<T>(this VariablesContainer collection, IIndexedVariable<T> variable)
        where T : notnull =>
        collection
            .TryGet(variable)
            .OrInvoke(() => throw new InvalidOperationException($"Failed to get {variable.Name} of type {typeof(T)} at index {variable.Index}"));

    public static IEnumerable<T> GetAll<T>(this VariablesContainer collection, IVariable<T> variable)
        where T : notnull
    {
        var index = 0;
        while (true)
        {
            var indexed = variable.WithIndex(index);
            var optional = collection.TryGet(indexed);
            if (!optional.TryGet(out var value))
            {
                break;
            }

            yield return value;
            index++;
        }
    }
}
