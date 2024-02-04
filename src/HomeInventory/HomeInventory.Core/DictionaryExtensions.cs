namespace HomeInventory.Core;

public static class DictionaryExtensions
{
    public static Optional<TValue> GetValueOptional<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull =>
        dictionary.TryGetValue(key, out var value) ? value : Optional.None<TValue>();

    public static Optional<TValue> GetValueOptional<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull =>
        dictionary.TryGetValue(key, out var value) ? value : Optional.None<TValue>();
}
