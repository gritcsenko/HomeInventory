namespace HomeInventory.Domain.Primitives;

public interface IEnumeration : IValueObject
{
}

public interface IEnumeration<TEnum> : IEnumeration, IValueObject<TEnum>
    where TEnum : IEnumeration<TEnum>
{
    static IReadOnlyCollection<TEnum> Items { get; } = null!;

    string Name { get; }
}

public interface IEnumeration<TEnum, out TValue> : IEnumeration<TEnum>
    where TEnum : IEnumeration<TEnum, TValue>
    where TValue : IEquatable<TValue>
{
    TValue Value { get; }
}
