namespace HomeInventory.Domain.Primitives;

public class AsyncDisposable : IAsyncDisposable
{
    public static IAsyncDisposable None { get; } = new AsyncDisposable();

    public bool IsDisposed { get; private set; }

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

    protected virtual ValueTask InternalDisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

}
