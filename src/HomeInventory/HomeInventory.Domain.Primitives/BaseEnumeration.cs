namespace HomeInventory.Domain.Primitives;

public abstract class BaseEnumeration<TSelf>(string name, object key) : ValueObject<TSelf>(name, key), IEnumeration<TSelf>
    where TSelf : BaseEnumeration<TSelf>
{
    private static readonly Lazy<EnumerationItemsCollection<TSelf>> _items = new(EnumerationItemsCollection.CreateFor<TSelf>, LazyThreadSafetyMode.ExecutionAndPublication);
    private readonly object _key = key;

    public string Name { get; } = name;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Interface implementation")]
    public static TSelf Parse(string text) =>
        TryParse(text)
            .OrThrow(() => throw new InvalidOperationException($"Failed to parse '{text}' to {typeof(TSelf).Name}"));

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Interface implementation")]
    public static Optional<TSelf> TryParse(string text) =>
        _items.Value.FirstOrNone(text);

    public override string ToString() => $"{Name} ({_key})";
}

public abstract class BaseEnumeration<TSelf, TValue>(string name, TValue value) : BaseEnumeration<TSelf>(name, value), IEnumeration<TSelf, TValue>
    where TSelf : BaseEnumeration<TSelf, TValue>
    where TValue : notnull, IEquatable<TValue>
{
    public TValue Value { get; } = value;
}
