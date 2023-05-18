namespace HomeInventory.Domain.Primitives;

public abstract class AsyncDisposable : IAsyncDisposable
{
    public static AsyncDisposable None { get; } = Create(() => ValueTask.CompletedTask);

    public bool IsDisposed { get; private set; }

    public static AsyncDisposable Create(Func<ValueTask> action) => new AsyncDisposeAction(action);

    public static AsyncDisposable Create<T>(Func<T, ValueTask> action, T state) => new AsyncDisposeAction<T>(action, state);

    public async ValueTask DisposeAsync()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
        await InternalDisposeAsync();
        GC.SuppressFinalize(this);
    }

    protected abstract ValueTask InternalDisposeAsync();
}
