using Disposable = System.Reactive.Disposables.Disposable;

namespace HomeInventory.Core;

public static class LazyExtensions
{
    public static IDisposable ToDisposable<TDisposable>(this Lazy<TDisposable> lazy)
        where TDisposable : notnull, IDisposable =>
        Disposable.Create(lazy.DisposeIfCreated);

    public static IAsyncDisposable ToAsyncDisposable<TDisposable>(this Lazy<TDisposable> lazy)
        where TDisposable : notnull, IAsyncDisposable =>
        new AnonymousAsyncDisposable(lazy.DisposeAsyncIfCreated);

    private static void DisposeIfCreated<T>(this Lazy<T> lazy)
        where T : IDisposable
    {
        if (lazy.IsValueCreated)
        {
            lazy.Value.Dispose();
        }
    }

    private static async ValueTask DisposeAsyncIfCreated<T>(this Lazy<T> lazy)
        where T : IAsyncDisposable
    {
        if (lazy.IsValueCreated)
        {
            await lazy.Value.DisposeAsync();
        }
    }
}
