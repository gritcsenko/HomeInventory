using FluentAssertions.Execution;

namespace HomeInventory.Tests.Framework;

public class ThenContext<TResult> : BaseContext
    where TResult : notnull
{
    private readonly IVariable<TResult> _resultVariable;

    public ThenContext(VariablesContainer variables, IVariable<TResult> resultVariable)
        : base(variables) =>
        _resultVariable = resultVariable;

    public ThenContext<TResult> Result(Action<TResult> assert)
    {
        using var scope = new AssertionScope();
        assert(GetResult());
        return this;
    }

    public ThenContext<TResult> Result<TArg>(Variable<TArg> arg, Action<TResult, TArg> assert)
        where TArg : notnull =>
        Result(arg.WithIndex(0), assert);

    public ThenContext<TResult> Result<TArg>(IndexedVariable<TArg> arg, Action<TResult, TArg> assert)
        where TArg : notnull =>
        Result(r => assert(r, Variables.Get(arg)));

    public ThenContext<TResult> Result<TArg1, TArg2>(Variable<TArg1> arg1, Variable<TArg2> arg2, Action<TResult, TArg1, TArg2> assert)
        where TArg1 : notnull
        where TArg2 : notnull =>
        Result(arg1.WithIndex(0), arg2.WithIndex(0), assert);

    public ThenContext<TResult> Result<TArg1, TArg2>(IndexedVariable<TArg1> arg1, IndexedVariable<TArg2> arg2, Action<TResult, TArg1, TArg2> assert)
        where TArg1 : notnull
        where TArg2 : notnull =>
        Result(r => assert(r, Variables.Get(arg1), Variables.Get(arg2)));

    private TResult GetResult()
    {
        var variable = _resultVariable.WithIndex(0);
        return Variables.Get(variable);
    }
}

public class ThenContext : BaseContext
{
    public ThenContext(VariablesContainer variables)
        : base(variables)
    {
    }

    public ThenContext Ensure(Action assert)
    {
        using var scope = new AssertionScope();
        assert();
        return this;
    }
}
