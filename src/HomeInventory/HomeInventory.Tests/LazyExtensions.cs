namespace HomeInventory.Tests;

public static class LazyExtensions
{
    public static void TryDispose<T>(this Lazy<T> lazy)
        where T : IDisposable
    {
        if (lazy.IsValueCreated)
        {
            lazy.Value.Dispose();
        }
    }
}
