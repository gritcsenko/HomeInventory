namespace HomeInventory.Domain.Primitives;

public abstract class BaseEnumeration<TSelf>(string name, object key) : ValueObject<TSelf>(name, key), IEnumeration<TSelf>
    where TSelf : BaseEnumeration<TSelf>
{
    private static readonly Lazy<EnumerationItemsCollection<TSelf>> _lazyItems = new(EnumerationItemsCollection.CreateFor<TSelf>, LazyThreadSafetyMode.ExecutionAndPublication);

    public string Name => GetComponent<string>(0);

    protected static IReadOnlyCollection<TSelf> Items => _lazyItems.Value;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Interface implementation")]
    public static Option<TSelf> TryParse(string text) => _lazyItems.Value[text];

    public override string ToString() => $"{GetComponent(0)} ({GetComponent(1)})";
}

public abstract class BaseEnumeration<TSelf, TValue>(string name, TValue value) : BaseEnumeration<TSelf>(name, value), IEnumeration<TSelf, TValue>
    where TSelf : BaseEnumeration<TSelf, TValue>
    where TValue : IEquatable<TValue>
{
    public TValue Value => GetComponent<TValue>(1);
}
