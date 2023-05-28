using DotNext;

namespace HomeInventory.Domain.Primitives;

public abstract class Enumeration<TSelf> : ValueObject<TSelf>, IEnumeration<TSelf>
    where TSelf : Enumeration<TSelf>
{
    private static readonly Lazy<EnumerationItemsCollection<TSelf>> _items = new(EnumerationItemsCollection.CreateFor<TSelf>, LazyThreadSafetyMode.ExecutionAndPublication);

    protected Enumeration(string name, object key)
        : base(name, key)
    {
        Name = name;
    }

    public string Name { get; }

    public static TSelf Parse(string text) =>
        TryParse(text)
            .OrThrow(() => throw new InvalidOperationException($"Failed to parse '{text}' to {typeof(TSelf).Name}"));

    public static Optional<TSelf> TryParse(string text) =>
        _items.Value.FirstOrNone(text);
}

public abstract class Enumeration<TSelf, TValue> : Enumeration<TSelf>, IEnumeration<TSelf, TValue>
    where TSelf : Enumeration<TSelf, TValue>
    where TValue : notnull, IEquatable<TValue>
{
    protected Enumeration(string name, TValue value)
        : base(name, value)
    {
        Value = value;
    }

    public TValue Value { get; }
}
