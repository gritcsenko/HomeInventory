namespace HomeInventory.Tests;

public class WhenContext : Context
{
    private readonly IVariable _resultVariable;

    public WhenContext(VariablesCollection variables, IVariable resultVariable)
        : base(variables) =>
        _resultVariable = resultVariable;

    public WhenContext Invoked<TSut, TResult>(IVariable<TSut> sut, Func<TSut, TResult> invoke)
        where TSut : notnull
        where TResult : notnull =>
        Invoked(sut.WithIndex(0), invoke);

    public WhenContext Invoked<TSut, TArg, TResult>(IVariable<TSut> sut, IVariable<TArg> arg, Func<TSut, TArg, TResult> invoke)
        where TSut : notnull
        where TArg : notnull
        where TResult : notnull =>
        Invoked(sut.WithIndex(0), arg.WithIndex(0), invoke);

    public WhenContext Invoked<TSut, TArg, TResult>(IIndexedVariable<TSut> sut, IIndexedVariable<TArg> arg, Func<TSut, TArg, TResult> invoke)
        where TSut : notnull
        where TArg : notnull
        where TResult : notnull
    {
        var argValue = Variables.Get(arg);
        return Invoked(sut, sutValue => invoke(sutValue, argValue));
    }

    public WhenContext Invoked<TSut, TResult>(IIndexedVariable<TSut> sut, Func<TSut, TResult> invoke)
        where TSut : notnull
        where TResult : notnull
    {
        var sutValue = Variables.Get(sut);
        Variables.Add(_resultVariable.OfType<TResult>(), () => invoke(sutValue));
        return this;
    }
}
