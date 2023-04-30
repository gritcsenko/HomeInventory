namespace HomeInventory.Tests;

internal static class EnumerableExtensions
{
    public static T FirstRandom<T>(this IReadOnlyCollection<T> source, int? seed = null)
    {
        var rnd = seed.HasValue ? new Random(seed.Value) : new Random();
        var index = rnd.Next(source.Count);
        return source.ElementAt(index);
    }

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
