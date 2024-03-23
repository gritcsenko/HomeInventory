namespace HomeInventory.Tests.Framework;

internal sealed class CancellationImplementation(CancellationTokenSource? source = null) : Disposable, ICancellation
{
    private readonly CancellationTokenSource _source = source ?? new CancellationTokenSource();

    public CancellationToken Token => _source.Token;

    public void Cancel() => _source.Cancel();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _source.Dispose();
        }
        base.Dispose(disposing);
    }
}
