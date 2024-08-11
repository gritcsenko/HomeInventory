namespace HomeInventory.Core;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> createValueFunc)
        where TKey : notnull =>
        dictionary.TryGetValue(key, out var value)
        ? value
        : dictionary.Add(key, createValueFunc);

    private static TValue Add<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> createValueFunc)
        where TKey : notnull
    {
        var newValue = createValueFunc(key);
        dictionary.Add(key, newValue);
        return newValue;
    }
}
