namespace HomeInventory.Core;

internal sealed class Scope<TContext> : IScope<TContext>
    where TContext : class
{
    private readonly AsyncLocal<Stack<Optional<TContext>>> _stack = new();

    public Optional<TContext> TryGet()
    {
        var stack = GetStack();
        return stack.TryPeek(out var context) ? context : Optional<TContext>.None;
    }

    public IDisposable Reset() => InternalSet(Optional<TContext>.None);

    public IDisposable Set(TContext context) => InternalSet(context);

    private IDisposable InternalSet(Optional<TContext> context)
    {
        var stack = GetStack();
        stack.Push(context);
        return System.Reactive.Disposables.Disposable.Create(() => stack.Pop());
    }

    private Stack<Optional<TContext>> GetStack() => _stack.Value ??= new Stack<Optional<TContext>>();
}
