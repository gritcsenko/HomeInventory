namespace HomeInventory.Core;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue createdValue)
        where TKey : notnull =>
        dictionary.GetOrAdd(key, _ => createdValue);

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> createValueFunc)
        where TKey : notnull =>
        dictionary.GetValueOptional(key).OrInvoke(() => dictionary[key] = createValueFunc(key));

    public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> defaultValue)
        where TKey : notnull =>
        dictionary.GetValueOptional(key).OrInvoke(() => defaultValue(key));

    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> defaultValue)
        where TKey : notnull =>
        dictionary.GetValueOptional(key).OrInvoke(() => defaultValue(key));

    public static Optional<TValue> GetValueOptional<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull =>
        dictionary?.TryGetValue(key, out var value) == true ? value : Optional.None<TValue>();

    public static Optional<TValue> GetValueOptional<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull =>
        dictionary?.TryGetValue(key, out var value) == true ? value : Optional.None<TValue>();
}
