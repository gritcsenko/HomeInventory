using FluentAssertions.Execution;

namespace HomeInventory.Tests.Framework;

public class ThenContext<TResult>(VariablesContainer variables, IVariable<TResult> resultVariable) : BaseContext(variables)
    where TResult : notnull
{
    private readonly IVariable<TResult> _resultVariable = resultVariable;

    public ThenContext<TResult> Result(Action<TResult> assert)
    {
        using var scope = new AssertionScope();
        var result = Variables.Get(_resultVariable);
        assert(result);
        return this;
    }

    public ThenContext<TResult> Result<TArg>(Variable<TArg> arg, Action<TResult, TArg> assert)
        where TArg : notnull =>
        Result(arg[0], assert);

    public ThenContext<TResult> Result<TArg>(IIndexedVariable<TArg> arg, Action<TResult, TArg> assert)
        where TArg : notnull =>
        Result(r => assert(r, Variables.Get(arg)));

    public ThenContext<TResult> Result<TArg1, TArg2>(Variable<TArg1> arg1, Variable<TArg2> arg2, Action<TResult, TArg1, TArg2> assert)
        where TArg1 : notnull
        where TArg2 : notnull =>
        Result(arg1[0], arg2[0], assert);

    public ThenContext<TResult> Result<TArg1, TArg2>(IIndexedVariable<TArg1> arg1, IIndexedVariable<TArg2> arg2, Action<TResult, TArg1, TArg2> assert)
        where TArg1 : notnull
        where TArg2 : notnull =>
        Result(r => assert(r, Variables.Get(arg1), Variables.Get(arg2)));
}

public class ThenContext(VariablesContainer variables) : BaseContext(variables)
{
    public ThenContext Ensure(Action assert)
    {
        using var scope = new AssertionScope();
        assert();
        return this;
    }
}
