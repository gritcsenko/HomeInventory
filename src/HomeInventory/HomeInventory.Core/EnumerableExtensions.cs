namespace HomeInventory.Core;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item) => ConcatCore(source, item);

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) => source ?? [];

    public static IReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> source) => source as IReadOnlyCollection<T> ?? source.ToArray();

    public static IEnumerable<T> WithCancellation<T>(this IEnumerable<T> source, CancellationToken cancellationToken) =>
        source.TakeWhile(_ => !cancellationToken.IsCancellationRequested);

    public static OptionAsync<T> FirstOrNoneAsync<T>(this IAsyncEnumerable<T> values, Func<T, bool> predicate, CancellationToken cancellationToken = default) =>
        values.Where(predicate).FirstOrNoneCoreAsync(cancellationToken).ToAsync();

    private static IEnumerable<T> ConcatCore<T>(IEnumerable<T> source, T item)
    {
        foreach (var i in source)
        {
            yield return i;
        }

        yield return item;
    }

    private static async Task<Option<T>> FirstOrNoneCoreAsync<T>(this IAsyncEnumerable<T> values, CancellationToken cancellationToken)
    {
        await using var enumerator = values.GetAsyncEnumerator(cancellationToken);
        return await enumerator.MoveNextAsync(cancellationToken)
            ? enumerator.Current
            : OptionNone.Default;
    }
}

public static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        switch (collection)
        {
            case List<T> list:
                list.AddRange(items);
                break;
            default:
                foreach (var item in items)
                {
                    collection.Add(item);
                }
                break;
        }
    }
}
