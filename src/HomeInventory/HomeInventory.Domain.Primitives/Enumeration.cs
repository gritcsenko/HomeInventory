using HomeInventory.Domain.Extensions;

namespace HomeInventory.Domain.Primitives;

public static class Enumeration
{
    internal static IEnumerable<TEnum> CollectItems<TEnum>()
        where TEnum : IEnumeration
    {
        var enumType = typeof(TEnum);
        var assembly = enumType.Assembly;
        var allTypes = assembly.GetTypes();
        var derivedTypes = allTypes.Where(enumType.IsAssignableFrom);
        var allFields = derivedTypes.SelectMany(t => t.GetFieldsOfType<TEnum>());
        return allFields;
    }
}

public abstract class Enumeration<TEnum, TKey> : ValueObject<TEnum>, IEnumeration<TEnum, TKey>
    where TEnum : Enumeration<TEnum, TKey>
    where TKey : IEquatable<TKey>
{
    private static readonly Lazy<List<TEnum>> LazyItems = new(() => Enumeration.CollectItems<TEnum>().ToList(), LazyThreadSafetyMode.ExecutionAndPublication);

    protected Enumeration(string name, TKey value)
        : base(name, value)
    {
        Name = name;
        Value = value;
    }

    public static IReadOnlyCollection<TEnum> Items => LazyItems.Value;

    public string Name { get; }

    public TKey Value { get; }
}

