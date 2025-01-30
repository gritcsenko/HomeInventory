
namespace HomeInventory.Tests.Framework;

public abstract class BaseContext(VariablesContainer variables)
{
    protected internal VariablesContainer Variables { get; } = variables;

    protected T GetValue<T>(IVariable<T> variable) =>
        GetValue(variable[0]);

    protected T GetValue<T>(IIndexedVariable<T> variable) =>
        Variables.Get(variable).Value;
}
