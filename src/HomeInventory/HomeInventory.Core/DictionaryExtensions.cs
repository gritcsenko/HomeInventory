using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HomeInventory.Core;

public static class DictionaryExtensions
{
    public static bool TryUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> createValueFunc)
        where TKey : notnull
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);
        if (Unsafe.IsNullRef(ref val))
        {
            return false;
        }

        val = createValueFunc(key);
        return true;
    }

    public static TResult GetOrAdd<TKey, TValue, TResult>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TResult> createValueFunc)
        where TKey : notnull
        where TResult : TValue
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
        if (!exists)
        {
            val = createValueFunc(key);
        }

        return (TResult)val!;
    }

    public static async ValueTask<TResult> GetOrAddAsync<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, Task<TResult>> createValueFunc)
        where TKey : notnull
        where TResult : TValue =>
        dictionary.TryGetValue(key, out var value)
            ? (TResult)value!
            : await dictionary.AddAsync(key, createValueFunc);

    private static async Task<TResult> AddAsync<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, Task<TResult>> createValueFunc)
        where TKey : notnull
        where TResult : TValue
    {
        var newValue = await createValueFunc(key);
        dictionary.Add(key, newValue);
        return newValue;
    }
}
