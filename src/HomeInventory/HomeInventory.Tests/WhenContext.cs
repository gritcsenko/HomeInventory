namespace HomeInventory.Tests;

public class WhenContext : Context
{
    private readonly IVariable _resultVariable;

    public WhenContext(VariablesCollection variables, IVariable resultVariable)
        : base(variables) =>
        _resultVariable = resultVariable;

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
}
