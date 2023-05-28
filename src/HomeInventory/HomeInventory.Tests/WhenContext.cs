using static HomeInventory.Tests.BaseTest;

namespace HomeInventory.Tests;

public class WhenContext : Context
{
    private readonly IVariable _resultVariable;
    private readonly ICancellation _cancellation;

    public WhenContext(VariablesCollection variables, IVariable resultVariable, ICancellation cancellation)
        : base(variables)
    {
        _resultVariable = resultVariable;
        _cancellation = cancellation;
    }

    public ThenCatchedContext Catched<TSut, TArg>(IVariable<TSut> sut, IVariable<TArg> arg, Action<TSut, TArg> invoke)
        where TSut : notnull
        where TArg : notnull =>
        Catched(sut.WithIndex(0), arg.WithIndex(0), invoke);

    public ThenCatchedContext Catched<TSut, TArg>(IIndexedVariable<TSut> sut, IIndexedVariable<TArg> arg, Action<TSut, TArg> invoke)
        where TSut : notnull
        where TArg : notnull =>
        Catched(sut, sutValue => invoke(sutValue, Variables.Get(arg)));

    public ThenContext<TResult> Invoked<TSut, TResult>(IVariable<TSut> sut, Func<TSut, TResult> invoke)
        where TSut : notnull
        where TResult : notnull =>
        Invoked(sut.WithIndex(0), invoke);

    public ThenContext<TResult> Invoked<TSut, TArg, TResult>(IVariable<TSut> sut, IVariable<TArg> arg, Func<TSut, TArg, TResult> invoke)
        where TSut : notnull
        where TArg : notnull
        where TResult : notnull =>
        Invoked(sut.WithIndex(0), arg.WithIndex(0), invoke);

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

    public ThenCatchedContext Catched<TSut>(IIndexedVariable<TSut> sut, Action<TSut> invoke)
        where TSut : notnull
    {
        var variable = _resultVariable.OfType<Action>();
        Action action = () => invoke(Variables.Get(sut));
        Variables.TryAdd(variable, () => action);
        return new(Variables, variable);
    }

    public async Task<ThenContext<TResult>> InvokedAsync<TSut, TArg, TResult>(IIndexedVariable<TSut> sut, IVariable<TArg> arg, Func<TSut, TArg, CancellationToken, Task<TResult>> invoke)
        where TSut : notnull
        where TArg : notnull
        where TResult : notnull =>
        await InvokedAsync(sut, (s, t) => invoke(s, Variables.Get(arg.WithIndex(0)), t));

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
