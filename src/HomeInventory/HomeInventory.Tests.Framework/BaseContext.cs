namespace HomeInventory.Tests.Framework;

public abstract class BaseContext(VariablesContainer variables)
{
    private readonly VariablesContainer _variables = variables;

    protected internal VariablesContainer Variables => _variables;

    protected T GetValue<T>(IVariable<T> variable)
        where T : notnull =>
        _variables.Get(variable[0]);
}
