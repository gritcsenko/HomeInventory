namespace HomeInventory.Core;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item) => source.Concat(Enumerable.Repeat(item, 1));

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source) => source.SelectMany(Func.Identity<IEnumerable<T>>());

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) => source ?? Enumerable.Empty<T>();

    public static IReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> source) => source as IReadOnlyCollection<T> ?? source.ToArray();

    public static IEnumerable<T> WithCancellation<T>(this IEnumerable<T> source, CancellationToken cancellationToken) =>
        source.TakeWhile(_ => !cancellationToken.IsCancellationRequested);

    public static Optional<T> FirstOrNone<T>(this IEnumerable<T> source)
    {
        foreach (var item in source)
        {
#pragma warning disable S1751 // Loops with at most one iteration should be refactored
            return item;
#pragma warning restore S1751 // Loops with at most one iteration should be refactored
        }

        return Optional<T>.None;
    }

    public static Optional<T> FirstOrNone<T>(this IEnumerable<T> source, Func<T, bool> filter)
    {
        foreach (var item in source.Where(item => filter(item)))
        {
#pragma warning disable S1751 // Loops with at most one iteration should be refactored
            return item;
#pragma warning restore S1751 // Loops with at most one iteration should be refactored
        }

        return Optional<T>.None;
    }
}
