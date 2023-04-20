namespace HomeInventory.Domain.Primitives;

public abstract class Enumeration<TEnum> : ValueObject<TEnum>, IEnumeration<TEnum>
    where TEnum : Enumeration<TEnum>
{
    private static readonly Lazy<TEnum[]> LazyItems = new(() => CollectItems().ToArray(), LazyThreadSafetyMode.ExecutionAndPublication);

    protected Enumeration(string name, object key)
        : base(name, key)
    {
        Name = name;
    }

    public static IReadOnlyCollection<TEnum> Items => LazyItems.Value;

    public string Name { get; }

    internal static IEnumerable<TEnum> CollectItems()
    {
        var enumType = typeof(TEnum);
        return enumType
            .Assembly
            .GetTypes()
            .Where(enumType.IsAssignableFrom)
            .SelectMany(t => t.GetFieldsOfType<TEnum>());
    }
}

public abstract class Enumeration<TEnum, TValue> : Enumeration<TEnum>, IEnumeration<TEnum, TValue>
    where TEnum : Enumeration<TEnum, TValue>
    where TValue : IEquatable<TValue>
{
    protected Enumeration(string name, TValue value)
        : base(name, value)
    {
        Value = value;
    }

    public TValue Value { get; }
}
