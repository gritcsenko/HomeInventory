namespace HomeInventory.Tests;

public class ThenContext : Context
{
    private readonly IVariable _resultVariable;

    public ThenContext(VariablesContainer variables, IVariable resultVariable)
        : base(variables) =>
        _resultVariable = resultVariable;

    public ThenContext Result<TResult>(Action<TResult> assert)
        where TResult : notnull
    {
        assert(GetResult<TResult>());
        return this;
    }

    public ThenContext Result<TResult, TArg>(IndexedVariable<TArg> arg, Action<TResult, TArg> assert)
        where TArg : notnull
        where TResult : notnull
    {
        assert(GetResult<TResult>(), Variables.Get(arg));
        return this;
    }

    private TResult GetResult<TResult>() where TResult : notnull
    {
        var variable = _resultVariable.OfType<TResult>().WithIndex(0);
        return Variables.Get(variable);
    }
}

public class ThenContext<TResult> : Context
    where TResult : notnull
{
    private readonly IVariable<TResult> _resultVariable;

    public ThenContext(VariablesContainer variables, IVariable<TResult> resultVariable)
        : base(variables) =>
        _resultVariable = resultVariable;

    public ThenContext<TResult> Result(Action<TResult> assert)
    {
        assert(GetResult());
        return this;
    }

    public ThenContext<TResult> Result<TArg>(Variable<TArg> arg, Action<TResult, TArg> assert)
        where TArg : notnull =>
        Result(arg.WithIndex(0), assert);

    public ThenContext<TResult> Result<TArg>(IndexedVariable<TArg> arg, Action<TResult, TArg> assert)
        where TArg : notnull
    {
        assert(GetResult(), Variables.Get(arg));
        return this;
    }

    private TResult GetResult()
    {
        var variable = _resultVariable.WithIndex(0);
        return Variables.Get(variable);
    }
}
