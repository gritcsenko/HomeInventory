using System.Runtime.InteropServices;

namespace HomeInventory.Core;

public static class DictionaryExtensions
{
    extension<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        where TKey : notnull
    {
        public TResult GetOrAdd<TResult>(TKey key, Func<TKey, TResult> createValueFunc)
            where TResult : TValue
        {
            ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
            if (exists)
            {
                return (TResult)val!;
            }

            var newValue = createValueFunc(key);
            dictionary[key] = newValue;
            return newValue;
        }

        public async ValueTask<TResult> GetOrAddAsync<TResult>(TKey key, Func<TKey, Task<TResult>> createValueFunc)
            where TResult : TValue
        {
            ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
            if (exists)
            {
                return (TResult)val!;
            }

            var newValue = await createValueFunc(key);
            dictionary[key] = newValue;
            return newValue;
        }
    }
}
