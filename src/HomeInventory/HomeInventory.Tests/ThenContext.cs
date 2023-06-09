namespace HomeInventory.Tests;

public class ThenContext<TResult> : BaseContext
    where TResult : notnull
{
    private readonly IVariable<TResult> _resultVariable;

    public ThenContext(VariablesContainer variables, IVariable<TResult> resultVariable)
        : base(variables) =>
        _resultVariable = resultVariable;

    public ThenContext<TResult> Result(Action<TResult> assert)
    {
        if (assert is null)
        {
            throw new ArgumentNullException(nameof(assert));
        }

        assert(GetResult());
        return this;
    }

    public ThenContext<TResult> Result<TArg>(Variable<TArg> arg, Action<TResult, TArg> assert)
        where TArg : notnull
    {
        if (arg is null)
        {
            throw new ArgumentNullException(nameof(arg));
        }

        return Result(arg.WithIndex(0), assert);
    }

    public ThenContext<TResult> Result<TArg>(IndexedVariable<TArg> arg, Action<TResult, TArg> assert)
        where TArg : notnull
    {
        if (assert is null)
        {
            throw new ArgumentNullException(nameof(assert));
        }

        assert(GetResult(), Variables.Get(arg));
        return this;
    }

    private TResult GetResult()
    {
        var variable = _resultVariable.WithIndex(0);
        return Variables.Get(variable);
    }
}
