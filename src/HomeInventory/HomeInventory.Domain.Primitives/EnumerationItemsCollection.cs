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

internal sealed class EnumerationItemsCollection<T>
    where T : IEnumeration<T>
{
    private readonly ILookup<string, T> _items;
    private readonly Lazy<IReadOnlyCollection<T>> _flattened;

    public EnumerationItemsCollection(IEnumerable<T> items)
    {
        _items = items.ToLookup(e => e.Name);
        _flattened = new(() => _items.Flatten().ToReadOnly());
    }

    public ILookup<string, T> AsLookup() => _items;

    public IReadOnlyCollection<T> AsReadOnly() => _flattened.Value;
}
