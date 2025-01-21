using System.Runtime.InteropServices;

namespace HomeInventory.Core;

public static class DictionaryExtensions
{
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

    public static async ValueTask<TResult> GetOrAddAsync<TKey, TValue, TResult>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, Task<TResult>> createValueFunc)
        where TKey : notnull
        where TResult : TValue
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
        if (!exists)
        {
            val = await createValueFunc(key);
        }

        return (TResult)val!;
    }
}
