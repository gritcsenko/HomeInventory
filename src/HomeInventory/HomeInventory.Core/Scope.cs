﻿namespace HomeInventory.Core;

internal sealed class Scope<TContext> : IScope<TContext>
    where TContext : class
{
    private readonly AsyncLocal<Stack<Option<TContext>>> _stack = new();

    public Option<TContext> Get()
    {
        var stack = GetStack();
        return stack.TryPeek(out var context) ? context : Option<TContext>.None;
    }

    public IDisposable Reset() => InternalSet(Option<TContext>.None);

    public IDisposable Set(TContext context) => InternalSet(context);

    private IDisposable InternalSet(Option<TContext> context)
    {
        var stack = GetStack();
        stack.Push(context);
        return System.Reactive.Disposables.Disposable.Create(() => stack.Pop());
    }

    private Stack<Option<TContext>> GetStack() => _stack.Value ??= new();
}
