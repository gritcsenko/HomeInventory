namespace HomeInventory.Domain.Primitives;

public abstract class BaseEnumeration<TSelf> : ValueObject<TSelf>, IEnumeration<TSelf>
    where TSelf : BaseEnumeration<TSelf>
{
    private static readonly Lazy<EnumerationItemsCollection<TSelf>> _lazyItems = new(EnumerationItemsCollection.CreateFor<TSelf>, LazyThreadSafetyMode.ExecutionAndPublication);

    protected BaseEnumeration(string name, object key)
        : base(name, key)
    {
    }

    public string Name => GetComponent<string>(0);

    protected static IReadOnlyCollection<TSelf> Items => _lazyItems.Value.AsReadOnly();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Interface implementation")]
    public static TSelf Parse(string text) =>
        TryParse(text)
            .OrThrow(() => throw new InvalidOperationException($"Failed to parse '{text}' to {typeof(TSelf).Name}"));

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Interface implementation")]
    public static Optional<TSelf> TryParse(string text) =>
        _lazyItems.Value.AsLookup()[text].FirstOrNone();

    public override string ToString() => $"{Name} ({GetComponent(1)})";
}

public abstract class BaseEnumeration<TSelf, TValue> : BaseEnumeration<TSelf>, IEnumeration<TSelf, TValue>
    where TSelf : BaseEnumeration<TSelf, TValue>
    where TValue : notnull, IEquatable<TValue>
{
    protected BaseEnumeration(string name, TValue value)
        : base(name, value)
    {
    }

    public TValue Value => GetComponent<TValue>(1);
}
