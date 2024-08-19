namespace HomeInventory.Core;

public static class DictionaryExtensions
{
    public static TResult GetOrAdd<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TResult> createValueFunc)
        where TKey : notnull
        where TResult : TValue =>
        dictionary.TryGetValue(key, out var value)
        ? (TResult)value!
        : dictionary.Add(key, createValueFunc);

    private static TResult Add<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TResult> createValueFunc)
        where TKey : notnull
        where TResult : TValue
    {
        var newValue = createValueFunc(key);
        dictionary.Add(key, newValue);
        return newValue;
    }
}
