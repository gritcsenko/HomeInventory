namespace HomeInventory.Tests.Framework;

public class BaseContext(VariablesContainer variables)
{
    private readonly VariablesContainer _variables = variables;

    protected VariablesContainer Variables => _variables;
}
