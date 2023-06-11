using FluentAssertions.Specialized;

namespace HomeInventory.Tests.Framework;

public class ThenCatchedContext : BaseContext
{
    private readonly IVariable<Action> _resultVariable;

    public ThenCatchedContext(VariablesContainer variables, IVariable<Action> resultVariable)
        : base(variables) =>
        _resultVariable = resultVariable;

    public void Exception<TException>(Action<ExceptionAssertions<TException>> assert)
        where TException : Exception =>
        assert(GetException<TException>());

    private ExceptionAssertions<TException> GetException<TException>()
       where TException : Exception
    {
        var variable = _resultVariable.WithIndex(0);
        var action = Variables.Get(variable);
        return action.Should().ThrowExactly<TException>();
    }
}
