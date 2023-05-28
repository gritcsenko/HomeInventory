using System.Collections;
using DotNext;
using DotNext.Collections.Generic;

namespace HomeInventory.Domain.Primitives;

internal static class EnumerationItemsCollection
{
    public static EnumerationItemsCollection<T> CreateFor<T>()
          where T : IEnumeration<T>
    {
        var items = CollectItems<T>();
        return new EnumerationItemsCollection<T>(items);
    }

    private static ILookup<string, T> CollectItems<T>()
        where T : IEnumeration<T> =>
        typeof(T)
            .GetFieldsOfType<T>()
            .ToLookup(e => e.Name);
}

internal sealed class EnumerationItemsCollection<T> : IReadOnlyCollection<T>
{
    private ILookup<string, T> _items;
    private Lazy<IReadOnlyCollection<T>> _flattened;

    public EnumerationItemsCollection(ILookup<string, T> items)
    {
        _items = items;
        _flattened = new(() => _items.Flatten().ToReadOnly());
    }

    public bool HasDuplicateNames => _items.Any(i => i.Count() > 1);

    public Optional<T> TryFind(string name) => _items[name].FirstOrNone();

    public int Count => _flattened.Value.Count;

    public IEnumerator<T> GetEnumerator() => _flattened.Value.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
