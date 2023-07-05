namespace HomeInventory.Core;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item) => source.Concat(Enumerable.Repeat(item, 1));

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source) => source.SelectMany(Func.Identity<IEnumerable<T>>());

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) => source ?? Array.Empty<T>();

    public static IReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> source) => source as IReadOnlyCollection<T> ?? source.ToArray();

    public static IEnumerable<T> WithCancellation<T>(this IEnumerable<T> source, CancellationToken cancellationToken) =>
        source.TakeWhile(_ => !cancellationToken.IsCancellationRequested);

    public static IAsyncEnumerable<T> DoAsync<T>(this IEnumerable<T> source, Func<T, ValueTask> asyncAction)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (asyncAction is null)
        {
            throw new ArgumentNullException(nameof(asyncAction));
        }

        return source.DoAsyncInternal(asyncAction);
    }

    private static async IAsyncEnumerable<T> DoAsyncInternal<T>(this IEnumerable<T> source, Func<T, ValueTask> asyncAction)
    {
        foreach (var item in source)
        {
            await asyncAction(item);
            yield return item;
        }
    }
}
