namespace HomeInventory.Core;

public static class DisposableExtensions
{
    public static IAsyncDisposable ToAsyncDisposable<T>(this T subject)
        where T : IDisposable
    {
        if (subject is IAsyncDisposable asyncDisposable)
        {
            return asyncDisposable;
        }

        return new AnonymousAsyncDisposable(() =>
        {
            subject.Dispose();
            return ValueTask.CompletedTask;
        });
    }
}
