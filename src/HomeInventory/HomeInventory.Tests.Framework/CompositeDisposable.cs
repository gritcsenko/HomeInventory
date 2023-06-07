namespace HomeInventory.Tests.Framework;

public abstract class CompositeDisposable : Disposable
{
    private readonly List<Action> _disposeActions = new();

    protected void AddDisposable<TDisposable>(TDisposable disposable)
        where TDisposable : notnull, IDisposable
    {
        _disposeActions.Add(disposable.Dispose);
    }

    protected void AddDisposable<TDisposable>(Lazy<TDisposable> lazyDisposable)
           where TDisposable : notnull, IDisposable
    {
        _disposeActions.Add(() => lazyDisposable.TryDispose());
    }

    protected sealed override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var action in _disposeActions)
            {
                action();
            }
        }

        base.Dispose(disposing);
    }
}
