namespace HomeInventory.Domain.Primitives;

public abstract class Disposable : IDisposable
{
    public static Disposable None { get; } = Create(() => { });

    public bool IsDisposed { get; private set; }

    public static Disposable Create(Action action) => new DisposeAction(action);

    public static Disposable Create<T>(Action<T> action, T state) => new DisposeAction<T>(action, state);

    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
        InternalDispose();
        GC.SuppressFinalize(this);
    }

    protected abstract void InternalDispose();
}
