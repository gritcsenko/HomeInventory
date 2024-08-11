namespace HomeInventory.Tests.Framework;

public class WhenContext(VariablesContainer variables, ICancellation cancellation) : BaseContext(variables)
{
    private readonly Variable _result = new(nameof(_result));
    private readonly ICancellation _cancellation = cancellation;

    internal ThenCatchedContext Catched<TSut, TArg>(IVariable<TSut> sut, IVariable<TArg> arg, Action<TSut, TArg> invoke)
        where TSut : notnull
        where TArg : notnull =>
        Catched(sut, sutValue => invoke(sutValue, GetValue(arg)));

    public ThenCatchedContext Catched<TSut>(IVariable<TSut> sut, Action<TSut> invoke)
        where TSut : notnull
    {
        var variable = _result.OfType<Action>();
        void action() => invoke(GetValue(sut));
        Variables.Add(variable, () => action);
        return new(Variables, variable);
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

    public ThenContext<TResult> Invoked<TSut, TResult>(IIndexedVariable<TSut> sut, Func<TSut, TResult> invoke)
        where TSut : notnull
        where TResult : notnull
    {
        var variable = _result.OfType<TResult>();
        Variables.Add(variable, () => invoke(Variables.Get(sut).Value));
        return new(Variables, variable);
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
