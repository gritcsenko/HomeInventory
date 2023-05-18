namespace HomeInventory.Domain.Primitives;

public class CompositeAsyncDisposable : AsyncDisposable
{
    private readonly List<IAsyncDisposable> _disposables = new();

    public CompositeAsyncDisposable(params IAsyncDisposable[] disposables)
        : this(disposables.AsEnumerable())
    {
    }

    public CompositeAsyncDisposable(IEnumerable<IAsyncDisposable> disposables)
    {
        _disposables.AddRange(disposables);
    }

    protected bool Remove(IAsyncDisposable disposable) => _disposables.Remove(disposable);

    protected override async ValueTask InternalDisposeAsync()
    {
        foreach (var disposable in _disposables)
        {
            await disposable.DisposeAsync();
        }
    }
}
