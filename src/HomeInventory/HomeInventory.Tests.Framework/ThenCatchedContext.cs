using FluentAssertions.Specialized;

namespace HomeInventory.Tests.Framework;

public class ThenCatchedContext(VariablesContainer variables, IVariable<Action> resultVariable) : BaseContext(variables)
{
    private readonly IVariable<Action> _resultVariable = resultVariable;

    public void Exception<TException>(Action<ExceptionAssertions<TException>> assert)
        where TException : Exception =>
        assert(GetException<TException>());

    private ExceptionAssertions<TException> GetException<TException>()
       where TException : Exception
    {
        var action = Variables.Get(_resultVariable);
        return action.Should().ThrowExactly<TException>();
    }
}
