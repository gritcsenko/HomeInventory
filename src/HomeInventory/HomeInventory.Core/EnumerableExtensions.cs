namespace HomeInventory.Core;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item) => ConcatCore(source, item);

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source) => source.SelectMany(Func.Identity<IEnumerable<T>>());

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) => source ?? [];

    public static IReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> source) => source as IReadOnlyCollection<T> ?? source.ToArray();

    public static IEnumerable<T> WithCancellation<T>(this IEnumerable<T> source, CancellationToken cancellationToken) =>
        source.TakeWhile(_ => !cancellationToken.IsCancellationRequested);

    public static Optional<T> FirstOrNone<T>(this IEnumerable<T> source, Func<T, bool> filter) => source.Where(filter).FirstOrNone();

    public static Optional<T> FirstOrNone<T>(this IEnumerable<T> source)
    {
        using var enumerator = source.GetEnumerator();
        return enumerator.MoveNext() ? enumerator.Current : Optional<T>.None;
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
