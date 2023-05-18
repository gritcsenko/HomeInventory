namespace HomeInventory.Domain.Primitives;

internal sealed class AsyncDisposeAction : AsyncDisposable
{
    private readonly Func<ValueTask> _action;

    public AsyncDisposeAction(Func<ValueTask> action) => _action = action;

    protected override async ValueTask InternalDisposeAsync() => await _action();
}

internal sealed class AsyncDisposeAction<T> : AsyncDisposable
{
    private readonly Func<T, ValueTask> _action;
    private readonly T _state;

    public AsyncDisposeAction(Func<T, ValueTask> action, T state) => (_action, _state) = (action, state);

    protected override async ValueTask InternalDisposeAsync() => await _action(_state);
}
