namespace HomeInventory.Tests.Framework;

public class WhenContext(VariablesContainer variables, IVariable resultVariable, ICancellation cancellation) : BaseContext(variables)
{
    private readonly IVariable _resultVariable = resultVariable;
    private readonly ICancellation _cancellation = cancellation;

    internal ThenCatchedContext Catched<TSut, TArg>(IVariable<TSut> sut, IVariable<TArg> arg, Action<TSut, TArg> invoke)
        where TSut : notnull
        where TArg : notnull =>
        Catched(sut[0], arg[0], invoke);

    public ThenCatchedContext Catched<TSut, TArg>(IIndexedVariable<TSut> sut, IIndexedVariable<TArg> arg, Action<TSut, TArg> invoke)
        where TSut : notnull
        where TArg : notnull =>
        Catched(sut, sutValue => invoke(sutValue, Variables.Get(arg)));

    public ThenCatchedContext Catched<TSut>(IIndexedVariable<TSut> sut, Action<TSut> invoke)
        where TSut : notnull
    {
        var variable = _resultVariable.OfType<Action>();
        void action() => invoke(Variables.Get(sut));
        Variables.TryAdd(variable, () => action);
        return new(Variables, variable);
    }

    internal ThenContext<TResult> Invoked<TSut, TResult>(IVariable<TSut> sut, Func<TSut, TResult> invoke)
        where TSut : notnull
        where TResult : notnull =>
        Invoked(sut[0], invoke);

    internal ThenContext<TResult> Invoked<TSut, TArg, TResult>(IVariable<TSut> sut, IVariable<TArg> arg, Func<TSut, TArg, TResult> invoke)
        where TSut : notnull
        where TArg : notnull
        where TResult : notnull =>
        Invoked(sut[0], arg[0], invoke);

    public ThenContext<TResult> Invoked<TSut, TArg, TResult>(IIndexedVariable<TSut> sut, IIndexedVariable<TArg> arg, Func<TSut, TArg, TResult> invoke)
        where TSut : notnull
        where TArg : notnull
        where TResult : notnull =>
        Invoked(sut, sutValue => invoke(sutValue, Variables.Get(arg)));

    public ThenContext<TResult> Invoked<TSut, TResult>(IIndexedVariable<TSut> sut, Func<TSut, TResult> invoke)
        where TSut : notnull
        where TResult : notnull
    {
        var variable = _resultVariable.OfType<TResult>();
        Variables.TryAdd(variable, () => invoke(Variables.Get(sut)));
        return new(Variables, variable);
    }

    public ThenContext InvokedForSideEffects<TSut>(IVariable<TSut> sut, Action<TSut> invoke)
        where TSut : notnull =>
        InvokedForSideEffects(sut[0], invoke);

    public ThenContext InvokedForSideEffects<TSut, TArg>(IVariable<TSut> sut, IVariable<TArg> arg, Action<TSut, TArg> invoke)
        where TSut : notnull
        where TArg : notnull =>
        InvokedForSideEffects(sut[0], arg[0], invoke);

    public ThenContext InvokedForSideEffects<TSut, TArg>(IIndexedVariable<TSut> sut, IIndexedVariable<TArg> arg, Action<TSut, TArg> invoke)
        where TSut : notnull
        where TArg : notnull =>
        InvokedForSideEffects(sut[0], x => invoke(x, Variables.Get(arg)));

    public ThenContext InvokedForSideEffects<TSut>(IIndexedVariable<TSut> sut, Action<TSut> invoke)
        where TSut : notnull
    {
        invoke(Variables.Get(sut));
        return new(Variables);
    }

    public async Task<ThenContext<TResult>> InvokedAsync<TSut, TArg, TResult>(IVariable<TSut> sut, IVariable<TArg> arg, Func<TSut, TArg, CancellationToken, Task<TResult>> invoke)
        where TSut : notnull
        where TArg : notnull
        where TResult : notnull =>
        await InvokedAsync(sut[0], arg[0], invoke);

    public async Task<ThenContext<TResult>> InvokedAsync<TSut, TArg, TResult>(IVariable<TSut> sut, IIndexedVariable<TArg> arg, Func<TSut, TArg, CancellationToken, Task<TResult>> invoke)
        where TSut : notnull
        where TArg : notnull
        where TResult : notnull =>
        await InvokedAsync(sut[0], arg, invoke);

    public async Task<ThenContext<TResult>> InvokedAsync<TSut, TArg, TResult>(IIndexedVariable<TSut> sut, IVariable<TArg> arg, Func<TSut, TArg, CancellationToken, Task<TResult>> invoke)
        where TSut : notnull
        where TArg : notnull
        where TResult : notnull =>
        await InvokedAsync(sut, arg[0], invoke);

    public async Task<ThenContext<TResult>> InvokedAsync<TSut, TArg, TResult>(IIndexedVariable<TSut> sut, IIndexedVariable<TArg> arg, Func<TSut, TArg, CancellationToken, Task<TResult>> invoke)
        where TSut : notnull
        where TArg : notnull
        where TResult : notnull =>
        await InvokedAsync(sut, (s, t) => invoke(s, Variables.Get(arg), t));

    public async Task<ThenContext<TResult>> InvokedAsync<TSut, TArg1, TArg2, TResult>(IVariable<TSut> sut, IVariable<TArg1> arg1, IVariable<TArg2> arg2, Func<TSut, TArg1, TArg2, CancellationToken, Task<TResult>> invoke)
        where TSut : notnull
        where TArg1 : notnull
        where TArg2 : notnull
        where TResult : notnull =>
        await InvokedAsync(sut[0], (s, t) => invoke(s, Variables.Get(arg1), Variables.Get(arg2), t));

    public async Task<ThenContext<TResult>> InvokedAsync<TSut, TArg1, TArg2, TResult>(IIndexedVariable<TSut> sut, IIndexedVariable<TArg1> arg1, IIndexedVariable<TArg2> arg2, Func<TSut, TArg1, TArg2, CancellationToken, Task<TResult>> invoke)
        where TSut : notnull
        where TArg1 : notnull
        where TArg2 : notnull
        where TResult : notnull =>
        await InvokedAsync(sut, (s, t) => invoke(s, Variables.Get(arg1), Variables.Get(arg2), t));

    public async Task<ThenContext<TResult>> InvokedAsync<TSut, TResult>(IIndexedVariable<TSut> sut, Func<TSut, CancellationToken, Task<TResult>> invoke)
        where TSut : notnull
        where TResult : notnull =>
        await InvokedAsync(t => invoke(Variables.Get(sut), t));

    public async Task<ThenContext<TResult>> InvokedAsync<TResult>(Func<CancellationToken, Task<TResult>> invoke)
        where TResult : notnull
    {
        var variable = _resultVariable.OfType<TResult>();
        await Variables.TryAddAsync(variable, () => invoke(_cancellation.Token));
        return new(Variables, variable);
    }
}
