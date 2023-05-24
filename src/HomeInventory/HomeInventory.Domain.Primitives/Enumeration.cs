using DotNext;

namespace HomeInventory.Domain.Primitives;

public abstract class Enumeration<TEnum> : ValueObject<TEnum>, IEnumeration<TEnum>
    where TEnum : Enumeration<TEnum>
{
    private static readonly Lazy<TEnum[]> _items = new(() => CollectItems().ToArray(), LazyThreadSafetyMode.ExecutionAndPublication);

    protected Enumeration(string name, object key)
        : base(name, key)
    {
        Name = name;
    }

    public static IReadOnlyCollection<TEnum> Items => _items.Value;

    public string Name { get; }

    private static IEnumerable<TEnum> CollectItems()
    {
        var enumType = typeof(TEnum);
        var assembly = enumType.Assembly;
        var allTypes = assembly.GetTypes();
        var derivedTypes = allTypes.Where(enumType.IsAssignableFrom);
        var allFields = derivedTypes.SelectMany(t => t.GetFieldsOfType<TEnum>());
        return allFields;
    }

    public static TEnum Parse(string name) =>
        TryParse(name)
            .OrThrow(() => throw new InvalidOperationException($"Failed to parse '{name}' to {typeof(TEnum).Name}"));

    public static Optional<TEnum> TryParse(string name)
    {
        foreach (var item in Items)
        {
            if (item.Name == name)
                return item;
        }

        return Optional.None<TEnum>();
    }
}

public abstract class Enumeration<TEnum, TValue> : Enumeration<TEnum>, IEnumeration<TEnum, TValue>
    where TEnum : Enumeration<TEnum, TValue>
    where TValue : notnull, IEquatable<TValue>
{
    protected Enumeration(string name, TValue value)
        : base(name, value)
    {
        Value = value;
    }

    public TValue Value { get; }
}
