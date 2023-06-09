using DotNext;

namespace HomeInventory.Domain.Primitives;

public abstract class BaseEnumeration<TSelf> : ValueObject<TSelf>, IEnumeration<TSelf>
    where TSelf : BaseEnumeration<TSelf>
{
    private static readonly Lazy<EnumerationItemsCollection<TSelf>> _items = new(EnumerationItemsCollection.CreateFor<TSelf>, LazyThreadSafetyMode.ExecutionAndPublication);
    private readonly object _key;

    protected BaseEnumeration(string name, object key)
        : base(name, key)
    {
        Name = name;
        _key = key;
    }

    public string Name { get; }

#pragma warning disable CA1000 // Do not declare static members on generic types
    public static TSelf Parse(string text) =>
        TryParse(text)
            .OrThrow(() => throw new InvalidOperationException($"Failed to parse '{text}' to {typeof(TSelf).Name}"));

    public static Optional<TSelf> TryParse(string text) =>
        _items.Value.FirstOrNone(text);
#pragma warning restore CA1000 // Do not declare static members on generic types

    public override string ToString() => $"{Name} ({_key})";
}

public abstract class BaseEnumeration<TSelf, TValue> : BaseEnumeration<TSelf>, IEnumeration<TSelf, TValue>
    where TSelf : BaseEnumeration<TSelf, TValue>
    where TValue : notnull, IEquatable<TValue>
{
    protected BaseEnumeration(string name, TValue value)
        : base(name, value)
    {
        Value = value;
    }

    public TValue Value { get; }
}
