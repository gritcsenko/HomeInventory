namespace HomeInventory.Core;

public static class CancellationTokenExtensions
{
    public static (CancellationToken token, IDisposable[] resources) WithTimeout(this CancellationToken sourceToken, TimeSpan timeout)
    {
        if (timeout == Timeout.InfiniteTimeSpan)
        {
            return (sourceToken, Array.Empty<IDisposable>());
        }

        var source = new CancellationTokenSource(timeout);
        var combined = CancellationTokenSource.CreateLinkedTokenSource(source.Token, sourceToken);
        return (combined.Token, new IDisposable[] { source, combined });
    }
}
