namespace HomeInventory.Domain;

internal static class LazyExtensions
{
    public static Lazy<T> ToLazy<T>(this Func<T> function) => new(function);
    public static Lazy<T> ToLazy<T>(this Func<T> function, bool isThreadSafe) => new(function, isThreadSafe);
}
