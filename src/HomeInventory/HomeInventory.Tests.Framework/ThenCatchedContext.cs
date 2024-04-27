using FluentAssertions.Specialized;

namespace HomeInventory.Tests.Framework;

public class ThenCatchedContext(VariablesContainer variables, IVariable<Action> actionVariable) : BaseContext(variables)
{
    private readonly IVariable<Action> _actionVariable = actionVariable;

    public void Exception<TException>(Action<ExceptionAssertions<TException>> assert)
        where TException : Exception =>
        assert(GetException<TException>());

    private ExceptionAssertions<TException> GetException<TException>()
       where TException : Exception
    {
        var action = GetValue(_actionVariable);
        return action.Should().ThrowExactly<TException>();
    }
}
