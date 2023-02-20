﻿namespace HomeInventory.Domain;

public static class DictionaryExtensions
{
    public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> defaultValueFunc)
        => dictionary.TryGetValue(key, out var value) ? value : defaultValueFunc(key);

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> getValueFunc)
        => dictionary.TryGetValue(key, out var value) ? value : dictionary[key] = getValueFunc(key);
}
