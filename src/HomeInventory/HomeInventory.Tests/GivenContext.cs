namespace HomeInventory.Tests;

public class GivenContext<TContext> : Context
    where TContext : GivenContext<TContext>
{
    public GivenContext(VariablesCollection variables, IFixture fixture)
        : base(variables)
    {
        Fixture = fixture;
    }

    public IFixture Fixture { get; }

    public TContext Create<T>(IVariable<T> variable)
        where T : notnull
    {
        return Add(variable, Fixture.Create<T>);
    }

    protected TContext Add<T>(IVariable<T> variable, Func<T> createValue)
        where T : notnull
    {
        if (!Variables.Add(variable, createValue))
        {
            throw new InvalidOperationException($"Failed to add variable '{variable.Name}' of type {typeof(T)}");
        }

        return (TContext)this;
    }
}
