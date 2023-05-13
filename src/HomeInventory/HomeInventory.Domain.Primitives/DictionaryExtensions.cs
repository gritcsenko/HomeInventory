namespace HomeInventory.Domain.Primitives;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue createdValue)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        return dictionary.GetOrAdd(key, _ => createdValue);
    }

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> createValueFunc)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        return dictionary.TryGetValue(key, out var existingValue)
            ? existingValue
            : dictionary[key] = createValueFunc(key);
    }

    public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> defaultValue)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        return dictionary.TryGetValue(key, out var value) ? value : defaultValue(key);
    }

    public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<Option<TKey>, TValue> dictionary, TKey key)
        where TKey : notnull
    {
        return dictionary.GetValueOrDefault(Option.Some(key), _ => dictionary[Option.None<TKey>()]);
    }

    public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<Option<TKey>, TValue> dictionary, TKey key, Func<TKey, TValue> defaultValue)
        where TKey : notnull
    {
        return dictionary.GetValueOrDefault(Option.Some(key), k => defaultValue(k.Reduce(key)));
    }
}
