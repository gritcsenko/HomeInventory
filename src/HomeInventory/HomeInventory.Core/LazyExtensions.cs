using Disposable = System.Reactive.Disposables.Disposable;

namespace HomeInventory.Core;

public static class LazyExtensions
{
    public static IDisposable ToDisposable<TDisposable>(this Lazy<TDisposable> lazy)
        where TDisposable : notnull, IDisposable =>
        Disposable.Create(lazy.DisposeIfCreated);

    private static void DisposeIfCreated<T>(this Lazy<T> lazy)
        where T : IDisposable
    {
        if (lazy.IsValueCreated)
        {
            lazy.Value.Dispose();
        }
    }
}
