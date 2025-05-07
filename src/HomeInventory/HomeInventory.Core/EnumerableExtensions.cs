namespace HomeInventory.Core;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (var item in collection)
        {
            action(item);
        }
    }

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item) => ConcatCore(source, item);

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) => source ?? [];

    public static IReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> source) => source as IReadOnlyCollection<T> ?? [.. source];

    public static IEnumerable<T> WithCancellation<T>(this IEnumerable<T> source, CancellationToken cancellationToken) =>
        source.TakeWhile(_ => !cancellationToken.IsCancellationRequested);

    public static async ValueTask<bool> AllAsync<T>(this IEnumerable<T> collection, Func<T, ValueTask<bool>> predicate)
    {
        foreach (var item in collection)
        {
            if (!await predicate(item))
            {
                return false;
            }
        }

        return true;
    }

    private static IEnumerable<T> ConcatCore<T>(IEnumerable<T> source, T item)
    {
        foreach (var i in source)
        {
            yield return i;
        }

        yield return item;
    }
}