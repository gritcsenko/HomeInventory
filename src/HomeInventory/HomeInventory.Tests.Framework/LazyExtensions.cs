namespace HomeInventory.Tests.Framework;

public static class LazyExtensions
{
    public static void TryDispose<T>(this Lazy<T> lazy)
        where T : IDisposable
    {
        if (lazy is null)
        {
            throw new ArgumentNullException(nameof(lazy));
        }

        if (lazy.IsValueCreated)
        {
            lazy.Value.Dispose();
        }
    }
}
