namespace HomeInventory.Infrastructure.Persistence;

internal sealed class EmptyAsyncDisposable : IAsyncDisposable
{
    private EmptyAsyncDisposable()
    {
    }

    public static IAsyncDisposable Instance { get; } = new EmptyAsyncDisposable();

    ValueTask IAsyncDisposable.DisposeAsync() => ValueTask.CompletedTask;
}
