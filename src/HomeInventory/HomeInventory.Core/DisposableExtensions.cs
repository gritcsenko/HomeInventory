namespace HomeInventory.Core;

public static class DisposableExtensions
{
    public static IAsyncDisposable ToAsyncDisposable<T>(this T subject)
        where T : IDisposable =>
        subject switch
        {
            IAsyncDisposable asyncDisposable => asyncDisposable,
            IDisposable disposable => new DisposableAdapter(disposable),
        };
}
