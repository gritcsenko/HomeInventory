namespace HomeInventory.Domain.Primitives;

public static class DictionaryExtensions
{
    public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> defaultValueFunc)
        where TKey : notnull =>
        dictionary.TryGetValue(key, out var value) ? value : defaultValueFunc(key);

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue createdValue)
        where TKey : notnull =>
        dictionary.GetOrAdd(key, _ => createdValue);

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> createValueFunc)
        where TKey : notnull =>
        dictionary.TryGetValue(key, out var existingValue)
            ? existingValue
            : dictionary[key] = createValueFunc(key);
}
