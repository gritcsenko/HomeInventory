namespace HomeInventory.Tests.Helpers;
internal static class EnumerableExtensions
{
    public static T FirstRandom<T>(this IReadOnlyCollection<T> source, int? seed = null)
    {
        var rnd = seed.HasValue ? new Random(seed.Value) : new Random();
        var index = rnd.Next(source.Count);
        return source.ElementAt(index);
    }

    public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T item) => source.Except(Enumerable.Repeat(item, 1));

    public static IEnumerable<T> Except<T>(this IEnumerable<T> source, params T[] items) => source.Except(items.AsEnumerable());

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item) => source.Concat(Enumerable.Repeat(item, 1));
}
