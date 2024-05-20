namespace HomeInventory.Core;

internal sealed class AnonymousAsyncDisposable(Func<ValueTask> dispose) : IAsyncDisposable
{
    private volatile Func<ValueTask>? _dispose = dispose;

    public async ValueTask DisposeAsync()
    {
        var func = Interlocked.Exchange(ref _dispose, null);
        if (func != null)
        {
            await func();
        }
    }
}
