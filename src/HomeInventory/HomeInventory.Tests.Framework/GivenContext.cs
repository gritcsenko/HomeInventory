namespace HomeInventory.Tests.Framework;

public class GivenContext<TContext>(VariablesContainer variables, IFixture fixture) : BaseContext(variables)
    where TContext : GivenContext<TContext>
{
    protected IFixture Fixture { get; } = fixture;

    protected TContext This => (TContext)this;

    public TContext New<T>(IVariable<T> variable)
        where T : notnull =>
        Add(variable, Fixture.Create<T>);

    public TContext New<T>(IVariable<T> variable, IVariable<int> countVariable)
        where T : notnull =>
        New(variable, countVariable[0]);

    public TContext New<T>(IVariable<T> variable, IIndexedVariable<int> countVariable)
        where T : notnull =>
        New(variable, Variables.Get(countVariable));

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

    public TContext SubstituteFor<T>(IVariable<T> variable, IVariable<int> countVariable, params Action<T, VariablesContainer>[] setups)
        where T : class =>
        SubstituteFor(variable, countVariable[0], setups);

    public TContext SubstituteFor<T>(IVariable<T> variable, IIndexedVariable<int> countVariable, params Action<T, VariablesContainer>[] setups)
        where T : class
    {
        foreach (var _ in Enumerable.Range(0, Variables.Get(countVariable)))
        {
            Add(variable, () =>
            {
                var value = Substitute.For<T>();
                foreach (var setup in setups)
                {
                    setup(value, Variables);
                }
                return value;
            });
        }

        return This;
    }

    public TContext Add<T>(IVariable<T> variable, T value)
        where T : notnull =>
        Add(variable, () => value);

    public TContext Add<T>(IVariable<T> variable, IVariable<int> countVariable, Func<T> createValue)
        where T : notnull =>
        Add(variable, countVariable[0], createValue);

    public TContext Add<T>(IVariable<T> variable, IIndexedVariable<int> countVariable, Func<T> createValue)
        where T : notnull
    {
        foreach (var _ in Enumerable.Range(0, Variables.Get(countVariable)))
        {
            Add(variable, createValue);
        }

        return This;
    }

    protected TContext Add<T>(IVariable<T> variable, Func<T> createValue)
        where T : notnull =>
        Variables.TryAdd(variable, createValue)
            ? This
            : throw new InvalidOperationException($"Failed to add variable '{variable.Name}' of type {typeof(T)}");

    public TContext AddAllToHashCode<T>(IVariable<HashCode> hash, IVariable<T> variable)
        where T : notnull =>
        AddAllToHashCode(hash[0], variable);

    public TContext AddAllToHashCode<T>(IIndexedVariable<HashCode> hash, IVariable<T> variable)
        where T : notnull
    {
        var hashValue = Variables.TryGet(hash)
            .OrInvoke(() => AddNewHashCode(hash));

        foreach (var value in Variables.GetMany(variable))
        {
            hashValue.Add(value);
        }

        return Variables.TryUpdate(hash, () => hashValue)
            ? This
            : throw new InvalidOperationException($"Failed to update variable '{hash.Name}'");
    }

    private HashCode AddNewHashCode(IIndexedVariable<HashCode> hash)
    {
        var value = new HashCode();
        Add(hash, () => value);
        return value;
    }
}
