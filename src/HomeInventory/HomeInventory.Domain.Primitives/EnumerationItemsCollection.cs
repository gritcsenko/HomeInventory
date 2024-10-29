using System.Collections;

namespace HomeInventory.Domain.Primitives;

internal static class EnumerationItemsCollection
{
    public static EnumerationItemsCollection<T> CreateFor<T>()
        where T : IEnumeration<T>
    {
        var items = typeof(T).GetFieldValuesOfType<T>();
        return new(items);
    }
}

internal sealed class EnumerationItemsCollection<T> : ISpannableCollection<T>
    where T : IEnumeration<T>
{
    private readonly ILookup<string, T> _items;
    private readonly Lazy<T[]> _flattened;

    public EnumerationItemsCollection(IEnumerable<T> items)
    {
        _items = items.ToLookup(e => e.Name);
        _flattened = new(() => _items.Flatten().ToArray());
    }

    public Option<T> this[string name] => _items[name].HeadOrNone();

    public Span<T> AsSpan() => new(_flattened.Value);

    public int Count => _flattened.Value.Length;

    public IEnumerator<T> GetEnumerator() => _flattened.Value.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
