namespace HomeInventory.Tests;

public class GivenContext<TContext> : Context
    where TContext : GivenContext<TContext>
{
    public GivenContext(VariablesContainer variables, IFixture fixture)
        : base(variables)
    {
        Fixture = fixture;
    }

    protected IFixture Fixture { get; }

    protected TContext This => (TContext)this;

    public TContext New<T>(IVariable<T> variable)
        where T : notnull =>
        Add(variable, Fixture.Create<T>);

    public TContext New<T>(IVariable<T> variable, int count)
        where T : notnull
    {
        foreach (var item in Fixture.CreateMany<T>(count))
        {
            Add(variable, () => item);
        }

        return This;
    }

    public TContext EmptyHashCode(IVariable<HashCode> hash) =>
        Add(hash, () => new HashCode());

    public TContext SubstituteFor<T>(IVariable<T> variable, params Action<T, VariablesContainer>[] setups)
        where T : class =>
        Add(variable, () =>
        {
            var value = Substitute.For<T>();
            foreach (var setup in setups)
            {
                setup(value, Variables);
            }
            return value;
        });

    protected TContext Add<T>(IVariable<T> variable, Func<T> createValue)
        where T : notnull
    {
        if (!Variables.TryAdd(variable, createValue))
        {
            throw new InvalidOperationException($"Failed to add variable '{variable.Name}' of type {typeof(T)}");
        }

        return This;
    }

    public TContext AddToHashCode<T>(IndexedVariable<HashCode> hash, IVariable<T> variable)
        where T : notnull =>
        AddToHashCode(hash, variable, 1);

    public TContext AddToHashCode<T>(IndexedVariable<HashCode> hash, IVariable<T> variable, int count)
        where T : notnull
    {
        var hashValue = Variables.TryGet(hash)
            .OrInvoke(() => AddNewHashCode(hash));

        foreach (var value in Variables.Get(variable, count))
        {
            hashValue.Add(value);
        }

        if (!Variables.TryUpdate(hash, () => hashValue))
        {
            throw new InvalidOperationException($"Failed to update variable '{hash.Name}'");
        }
        return This;
    }

    private HashCode AddNewHashCode(IndexedVariable<HashCode> hash)
    {
        var value = new HashCode();
        Add(hash, () => value);
        return value;
    }
}
