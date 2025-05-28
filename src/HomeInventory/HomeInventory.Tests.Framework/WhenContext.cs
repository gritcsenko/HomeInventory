namespace HomeInventory.Tests.Framework;

public class WhenContext(VariablesContainer variables, ICancellation cancellation) : BaseContext(variables)
{
    private readonly Variable _result = new(nameof(_result));
    private readonly ICancellation _cancellation = cancellation;

    internal ThenCaughtContext Caught<TSut, TArg>(IVariable<TSut> sut, IVariable<TArg> arg, Action<TSut, TArg> invoke)
        where TSut : notnull
        where TArg : notnull =>
        Caught(sut, sutValue => invoke(sutValue, GetValue(arg)));

    public ThenCaughtContext Caught<TSut>(IVariable<TSut> sut, Action<TSut> invoke)
        where TSut : notnull
    {
        var variable = _result.OfType<Action>();
        Variables.Add(variable, () => Action);
        return new(Variables, variable);

        void Action() => invoke(GetValue(sut));
    }

    internal ThenContext Invoked<TSut>(IVariable<TSut> sut, Action<TSut> invoke)
        where TSut : notnull
    {
        invoke(GetValue(sut));
        return new(Variables);
    }

    internal ThenContext Invoked<TSut, TArg>(IVariable<TSut> sut, IVariable<TArg> arg, Action<TSut, TArg> invoke)
        where TSut : notnull
        where TArg : notnull
    {
        invoke(GetValue(sut), GetValue(arg));
        return new(Variables);
    }

    internal ThenContext Invoked<TSut, TArg1, TArg2>(IVariable<TSut> sut, IVariable<TArg1> arg1, IVariable<TArg2> arg2, Action<TSut, TArg1, TArg2> invoke)
        where TSut : notnull
        where TArg1 : notnull
        where TArg2 : notnull
    {
        invoke(GetValue(sut), GetValue(arg1), GetValue(arg2));
        return new(Variables);
    }

    internal ThenContext Invoked<TSut, TArg1, TArg2, TArg3>(IVariable<TSut> sut, IVariable<TArg1> arg1, IVariable<TArg2> arg2, IVariable<TArg3> arg3, Action<TSut, TArg1, TArg2, TArg3> invoke)
        where TSut : notnull
        where TArg1 : notnull
        where TArg2 : notnull
        where TArg3 : notnull
    {
        invoke(GetValue(sut), GetValue(arg1), GetValue(arg2), GetValue(arg3));
        return new(Variables);
    }

    internal ThenContext Invoked<TSut, TArg1, TArg2, TArg3, TArg4>(IVariable<TSut> sut, IVariable<TArg1> arg1, IVariable<TArg2> arg2, IVariable<TArg3> arg3, IVariable<TArg4> arg4, Action<TSut, TArg1, TArg2, TArg3, TArg4> invoke)
        where TSut : notnull
        where TArg1 : notnull
        where TArg2 : notnull
        where TArg3 : notnull
        where TArg4 : notnull
    {
        invoke(GetValue(sut), GetValue(arg1), GetValue(arg2), GetValue(arg3), GetValue(arg4));
        return new(Variables);
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
        Invoked(sut, sutValue => invoke(sutValue, Variables.Get(arg).Value));

    internal ThenContext<TResult> Invoked<TSut, TArg1, TArg2, TResult>(IVariable<TSut> sut, IVariable<TArg1> arg1, IVariable<TArg2> arg2, Func<TSut, TArg1, TArg2, TResult> invoke)
        where TSut : notnull
        where TArg1 : notnull
        where TArg2 : notnull
        where TResult : notnull =>
        Invoked(sut[0], arg1[0], arg2[0], invoke);

    public ThenContext<TResult> Invoked<TSut, TArg1, TArg2, TResult>(IIndexedVariable<TSut> sut, IIndexedVariable<TArg1> arg1, IIndexedVariable<TArg2> arg2, Func<TSut, TArg1, TArg2, TResult> invoke)
        where TSut : notnull
        where TArg1 : notnull
        where TArg2 : notnull
        where TResult : notnull =>
        Invoked(sut, sutValue => invoke(sutValue, Variables.Get(arg1).Value, Variables.Get(arg2).Value));

    public ThenContext<TResult> Invoked<TSut, TResult>(IIndexedVariable<TSut> sut, Func<TSut, TResult> invoke)
        where TSut : notnull
        where TResult : notnull
    {
        var variable = _result.OfType<TResult>();
        Variables.Add(variable, () => invoke(Variables.Get(sut).Value));
        return new(Variables, variable);
    }

    public async Task<ThenContext> InvokedAsync<TSut, TArg>(IVariable<TSut> sut, IVariable<TArg> arg, Func<TSut, TArg, CancellationToken, Task> invoke)
        where TSut : notnull
        where TArg : notnull =>
        await InvokedAsync(sut[0], arg[0], invoke);

    public async Task<ThenContext> InvokedAsync<TSut, TArg>(IIndexedVariable<TSut> sut, IIndexedVariable<TArg> arg, Func<TSut, TArg, CancellationToken, Task> invoke)
        where TSut : notnull
        where TArg : notnull =>
        await InvokedAsync(sut, (s, t) => invoke(s, GetValue(arg), t));

    public async Task<ThenContext> InvokedAsync<TSut>(IIndexedVariable<TSut> sut, Func<TSut, CancellationToken, Task> invoke)
        where TSut : notnull =>
        await InvokedAsync(t => invoke(GetValue(sut), t));

    public async Task<ThenContext> InvokedAsync(Func<CancellationToken, Task> invoke)
    {
        await invoke(_cancellation.Token);
        return new(Variables);
    }

    public async Task<ThenContext<TResult>> InvokedAsync<TSut, TArg, TResult>(IVariable<TSut> sut, IVariable<TArg> arg, Func<TSut, TArg, CancellationToken, Task<TResult>> invoke)
        where TSut : notnull
        where TArg : notnull
        where TResult : notnull =>
        await InvokedAsync(sut[0], arg[0], invoke);

    public async Task<ThenContext<TResult>> InvokedAsync<TSut, TArg, TResult>(IIndexedVariable<TSut> sut, IIndexedVariable<TArg> arg, Func<TSut, TArg, CancellationToken, Task<TResult>> invoke)
        where TSut : notnull
        where TArg : notnull
        where TResult : notnull =>
        await InvokedAsync(sut, (s, t) => invoke(s, GetValue(arg), t));

    public async Task<ThenContext<TResult>> InvokedAsync<TSut, TArg1, TArg2, TResult>(IVariable<TSut> sut, IVariable<TArg1> arg1, IVariable<TArg2> arg2, Func<TSut, TArg1, TArg2, CancellationToken, Task<TResult>> invoke)
        where TSut : notnull
        where TArg1 : notnull
        where TArg2 : notnull
        where TResult : notnull =>
        await InvokedAsync(sut[0], (s, t) => invoke(s, GetValue(arg1), GetValue(arg2), t));

    public async Task<ThenContext<TResult>> InvokedAsync<TSut, TResult>(IIndexedVariable<TSut> sut, Func<TSut, CancellationToken, Task<TResult>> invoke)
        where TSut : notnull
        where TResult : notnull =>
        await InvokedAsync(t => invoke(GetValue(sut), t));

    public async Task<ThenContext<TResult>> InvokedAsync<TResult>(Func<CancellationToken, Task<TResult>> invoke)
        where TResult : notnull
    {
        var variable = _result.OfType<TResult>();
        await Variables.AddAsync(variable, () => invoke(_cancellation.Token));
        return new(Variables, variable);
    }
}
