using FluentResults;

namespace HomeInventory.Domain.Extensions;

public static class DictionaryExtensions
{
    public static Result<TValue> GetValueOrFail<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, IError> createErrorFunc)
        => dictionary.TryGetValue(key, out var value) ? Result.Ok(value) : Result.Fail<TValue>(createErrorFunc(key));
}