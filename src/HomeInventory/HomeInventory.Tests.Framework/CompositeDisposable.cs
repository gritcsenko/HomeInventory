namespace HomeInventory.Tests.Framework;

public class CompositeDisposable : Disposable
{
    private readonly List<Action> _disposeActions = new();

    public TDisposable AddDisposable<TDisposable>(TDisposable disposable)
        where TDisposable : notnull, IDisposable
    {
        AddDisposable(disposable.Dispose);
        return disposable;
    }

    public void AddDisposable<TDisposable>(Lazy<TDisposable> lazyDisposable)
        where TDisposable : notnull, IDisposable =>
        AddDisposable(() => lazyDisposable.TryDispose());

    public void AddDisposable(Action action) =>
        _disposeActions.Add(action);

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
