namespace HomeInventory.Tests.Framework;

public class CompositeDisposable : Disposable
{
    private readonly List<IDisposable> _disposables = [];

    public void AddDisposable<TDisposable>(TDisposable disposable, out TDisposable result)
        where TDisposable : notnull, IDisposable =>
        AddDisposable(() => disposable, out result);

    public void AddDisposable<TDisposable>(out TDisposable result)
        where TDisposable : notnull, IDisposable, new() =>
        AddDisposable(() => new(), out result);

    public void AddDisposable<TDisposable>(Func<TDisposable> disposableFunc, out TDisposable result)
        where TDisposable : notnull, IDisposable
    {
        result = disposableFunc();
        AddDisposable(result);
    }

    public void AddDisposable<TDisposable>(Lazy<TDisposable> lazyDisposable)
        where TDisposable : notnull, IDisposable =>
        AddDisposable(new DisposableAction(lazyDisposable.DisposeIfCreated));

    public void AddDisposable(IDisposable disposable) =>
        _disposables.Add(disposable);

    protected sealed override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
            _disposables.Clear();
        }

        base.Dispose(disposing);
    }
}
