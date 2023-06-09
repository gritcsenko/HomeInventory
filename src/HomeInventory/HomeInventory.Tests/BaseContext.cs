namespace HomeInventory.Tests;

public class BaseContext
{
    private readonly VariablesContainer _variables;

    public BaseContext(VariablesContainer variables)
    {
        _variables = variables;
    }

    protected VariablesContainer Variables => _variables;
}
