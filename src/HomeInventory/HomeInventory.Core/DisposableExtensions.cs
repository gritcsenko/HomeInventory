namespace HomeInventory.Core;

public static class DisposableExtensions
{
    extension<T>(T subject)
        where T : IDisposable
    {
        public IAsyncDisposable ToAsyncDisposable() =>
            subject switch
            {
                IAsyncDisposable asyncDisposable => asyncDisposable,
                IDisposable disposable => new SyncToAsyncDisposableAdapter(disposable),
            };
    }
}
