namespace HomeInventory.Tests;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item) => source.Concat(Enumerable.Repeat(item, 1));

    public static async IAsyncEnumerable<T> DoAsync<T>(this IEnumerable<T> source, Func<T, ValueTask> asyncAction)
    {
        foreach (var item in source)
        {
            await asyncAction(item);
            yield return item;
        }
    }

    public static async IAsyncEnumerable<T> DoAsync<T>(this IAsyncEnumerable<T> source, Func<T, ValueTask> asyncAction)
    {
        await foreach (var item in source)
        {
            await asyncAction(item);
            yield return item;
        }
    }
}
