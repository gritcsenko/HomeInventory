namespace HomeInventory.Tests.Framework;

public class ThenContext<TResult>(VariablesContainer variables, IVariable<TResult> resultVariable) : ThenContext(variables)
    where TResult : notnull
{
    private readonly IVariable<TResult> _resultVariable = resultVariable;

    public ThenContext<TResult> Result(Action<TResult> assert)
    {
        using var scope = new AssertionScope();
        var result = GetValue(_resultVariable);
        assert(result);
        return this;
    }

    public ThenContext<TResult> Result<TArg>(IVariable<TArg> arg, Action<TResult, TArg> assert)
        where TArg : notnull =>
        Result(arg[0], assert);

    public ThenContext<TResult> Result<TArg>(IIndexedVariable<TArg> arg, Action<TResult, TArg> assert)
        where TArg : notnull =>
        Result(r => assert(r, GetValue(arg)));

    public ThenContext<TResult> Result<TArg1, TArg2>(IVariable<TArg1> arg1, IVariable<TArg2> arg2, Action<TResult, TArg1, TArg2> assert)
        where TArg1 : notnull
        where TArg2 : notnull =>
        Result(arg1[0], arg2[0], assert);

    public ThenContext<TResult> Result<TArg1, TArg2>(IIndexedVariable<TArg1> arg1, IIndexedVariable<TArg2> arg2, Action<TResult, TArg1, TArg2> assert)
        where TArg1 : notnull
        where TArg2 : notnull =>
        Result(r => assert(r, GetValue(arg1), GetValue(arg2)));

    public ThenContext<TResult> Result<TArg1, TArg2, TArg3>(IVariable<TArg1> arg1, IVariable<TArg2> arg2, IVariable<TArg3> arg3, Action<TResult, TArg1, TArg2, TArg3> assert)
        where TArg1 : notnull
        where TArg2 : notnull
        where TArg3 : notnull =>
        Result(arg1[0], arg2[0], arg3[0], assert);

    public ThenContext<TResult> Result<TArg1, TArg2, TArg3>(IIndexedVariable<TArg1> arg1, IIndexedVariable<TArg2> arg2, IIndexedVariable<TArg3> arg3, Action<TResult, TArg1, TArg2, TArg3> assert)
        where TArg1 : notnull
        where TArg2 : notnull
        where TArg3 : notnull =>
        Result(r => assert(r, GetValue(arg1), GetValue(arg2), GetValue(arg3)));
}

public class ThenContext(VariablesContainer variables) : BaseContext(variables)
{
    public ThenContext Ensure<TArg1, TArg2, TArg3>(IVariable<TArg1> arg1Variable, IVariable<TArg2> arg2Variable, IVariable<TArg3> arg3Variable, Action<TArg1, TArg2, TArg3> assert)
        where TArg1 : notnull
        where TArg2 : notnull
        where TArg3 : notnull =>
        Ensure(arg1Variable, arg2Variable, (arg1, arg2) => assert(arg1, arg2, GetValue(arg3Variable)));

    public ThenContext Ensure<TArg1, TArg2>(IVariable<TArg1> arg1Variable, IVariable<TArg2> arg2Variable, Action<TArg1, TArg2> assert)
        where TArg1 : notnull
        where TArg2 : notnull =>
        Ensure(arg1Variable, arg1 => assert(arg1, GetValue(arg2Variable)));

    public ThenContext Ensure<TArg1>(IVariable<TArg1> argVariable, Action<TArg1> assert)
        where TArg1 : notnull =>
        Ensure(() => assert(GetValue(argVariable)));

    public ThenContext Ensure(Action assert)
    {
        using var scope = new AssertionScope();
        assert();
        return this;
    }
}
