namespace HomeInventory.Domain.Primitives;

public interface IEnumeration<TSelf> : IValueObject<TSelf>, IParseableValue<TSelf>
    where TSelf : notnull, IEnumeration<TSelf>
{
    string Name { get; }
}

public interface IEnumeration<TSelf, out TValue> : IEnumeration<TSelf>
    where TSelf : IEnumeration<TSelf, TValue>
    where TValue : IEquatable<TValue>
{
    TValue Value { get; }
}
