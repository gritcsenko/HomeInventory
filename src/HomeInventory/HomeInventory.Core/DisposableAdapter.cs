namespace HomeInventory.Core;

internal sealed class DisposableAdapter(IDisposable disposable) : IAsyncDisposable
{
    private readonly IDisposable _disposable = disposable;
    public ValueTask DisposeAsync()
    {
        _disposable.Dispose();
        return ValueTask.CompletedTask;
    }
}