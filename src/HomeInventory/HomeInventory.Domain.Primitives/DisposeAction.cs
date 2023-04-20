namespace HomeInventory.Domain.Primitives;

public class DisposeAction : Disposable
{
    private readonly Action _action;

    public DisposeAction(Action action)
    {
        _action = action;
    }

    protected override void InternalDispose() => _action();
}

public class DisposeAction<T> : Disposable
{
    private readonly Action<T> _action;
    private readonly T _state;

    public DisposeAction(Action<T> action, T state)
    {
        _action = action;
        _state = state;
    }

    protected override void InternalDispose() => _action(_state);
}
