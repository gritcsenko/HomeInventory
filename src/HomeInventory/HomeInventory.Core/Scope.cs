namespace HomeInventory.Core;

internal sealed class Scope<TContext> : IScope<TContext>
    where TContext : class
{
    private readonly AsyncLocal<Stack<TContext?>> _stack = new();

    public TContext? Get()
    {
        var stack = GetStack();
        return stack.TryPeek(out var context) ? context : null;
    }

    public IDisposable Reset() => InternalSet(null);

    public IDisposable Set(TContext context) => InternalSet(context);

    private IDisposable InternalSet(TContext? context)
    {
        var stack = GetStack();
        stack.Push(context);
        return System.Reactive.Disposables.Disposable.Create(() => stack.Pop());
    }

    private Stack<TContext?> GetStack() => _stack.Value ??= new Stack<TContext?>();
}
