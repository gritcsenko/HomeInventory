using System.Collections;

namespace HomeInventory.Domain.Primitives;

internal static class EnumerationItemsCollection
{
    public static EnumerationItemsCollection<T> CreateFor<T>()
        where T : IEnumeration<T>
    {
        var items = typeof(T).GetFieldsOfType<T>();
        return new EnumerationItemsCollection<T>(items);
    }
}

internal sealed class EnumerationItemsCollection<T> : IReadOnlyCollection<T>
    where T : IEnumeration<T>
{
    private readonly ILookup<string, T> _items;
    private readonly Lazy<IReadOnlyCollection<T>> _flattened;

    public EnumerationItemsCollection(IEnumerable<T> items)
    {
        _items = items.ToLookup(e => e.Name);
        _flattened = new(() => _items.Flatten().ToReadOnly());
    }

    public Optional<T> FirstOrNone(string name) => _items[name].FirstOrNone();

    public int Count => _flattened.Value.Count;

    public IEnumerator<T> GetEnumerator() => _flattened.Value.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
