namespace HomeInventory.Domain.Primitives;

public class Disposable : IDisposable
{
    public static IDisposable None { get; } = new Disposable();

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

    protected virtual void InternalDispose()
    {
    }
}
