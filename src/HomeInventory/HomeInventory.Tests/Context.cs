namespace HomeInventory.Tests;

public class Context
{
    private readonly VariablesContainer _variables;

    public Context(VariablesContainer variables)
    {
        _variables = variables;
    }

    protected VariablesContainer Variables => _variables;
}
