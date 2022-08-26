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

public abstract class Enumeration<TEnum, TKey> : ValueObject<TEnum>, IEnumeration<TEnum>
    where TEnum : Enumeration<TEnum, TKey>
    where TKey : notnull
{
    private static readonly Lazy<List<TEnum>> _items = new(() => Enumeration.CollectItems<TEnum>().ToList(), LazyThreadSafetyMode.ExecutionAndPublication);

    private readonly string _name;
    private readonly TKey _value;

    protected Enumeration(string name, TKey value)
    {
        _name = name;
        _value = value;
    }

    public static IReadOnlyCollection<TEnum> Items => _items.Value;

    public string Name => _name;

    public TKey Value => _value;

    protected static void Add(TEnum item) => _items.Value.Add(item);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
        yield return _name;
    }
}

