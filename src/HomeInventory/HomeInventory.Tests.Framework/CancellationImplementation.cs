namespace HomeInventory.Tests.Framework;

internal sealed class CancellationImplementation(CancellationTokenSource? source = null) : ICancellation, IDisposable
{
    private readonly CancellationTokenSource _source = source ?? new CancellationTokenSource();

    public CancellationToken Token => _source.Token;

    public void Cancel() => _source.Cancel();

    public void Dispose() => _source.Dispose();
}
