namespace HomeInventory.Domain.Extensions;
public static class EnumerableExtensions
{
    public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T item) => source.Except(Enumerable.Repeat(item, 1));

    public static IEnumerable<T> Except<T>(this IEnumerable<T> source, params T[] items) => source.Except(items.AsEnumerable());

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item) => source.Concat(Enumerable.Repeat(item, 1));
}
