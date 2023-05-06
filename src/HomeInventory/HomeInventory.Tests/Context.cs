﻿namespace HomeInventory.Tests;

public class Context
{
    private readonly VariablesCollection _variables;

    public Context(VariablesCollection variables)
    {
        _variables = variables;
    }

    protected VariablesCollection Variables => _variables;
}
