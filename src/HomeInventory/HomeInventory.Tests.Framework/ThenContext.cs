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

    private TResult GetResult()
    {
        var variable = _resultVariable.WithIndex(0);
        return Variables.Get(variable);
    }
}
