using HomeInventory.Domain.Primitives;

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

    public TContext New<T>(IVariable<T> variable)
        where T : notnull
    {
        return Add(variable, Fixture.Create<T>);
    }

    public TContext New<T>(IVariable<T> variable, int count)
        where T : notnull
    {
        foreach (var item in Fixture.CreateMany<T>(count))
        {
            Add(variable, () => item);
        }

        return (TContext)this;
    }

    public TContext EmptyHashCode(IVariable<HashCode> hash) =>
        Add(hash, () => new HashCode());

    protected TContext Add<T>(IVariable<T> variable, Func<T> createValue)
        where T : notnull
    {
        if (!Variables.TryAdd(variable, createValue))
        {
            throw new InvalidOperationException($"Failed to add variable '{variable.Name}' of type {typeof(T)}");
        }

        return (TContext)this;
    }

    public TContext AddToHashCode<T>(IndexedVariable<HashCode> hash, IndexedVariable<T> variable)
        where T : notnull
    {
        var hashValue = Variables.TryGet(hash)
            .Reduce(() => AddNewHashCode(hash));

        hashValue.Add(Variables.Get(variable));

        if (!Variables.TryUpdate(hash, () => hashValue))
        {
            throw new InvalidOperationException($"Failed to update variable '{hash.Name}'");
        }
        return (TContext)this;
    }

    public TContext AddToHashCode<T>(IndexedVariable<HashCode> hash, IVariable<T> variable, int count)
        where T : notnull
    {
        var hashValue = Variables.TryGet(hash)
            .Reduce(() => AddNewHashCode(hash));

        foreach (var value in Variables.Get(variable, count))
        {
            hashValue.Add(value);
        }

        if (!Variables.TryUpdate(hash, () => hashValue))
        {
            throw new InvalidOperationException($"Failed to update variable '{hash.Name}'");
        }
        return (TContext)this;
    }

    private HashCode AddNewHashCode(IndexedVariable<HashCode> hash)
    {
        var value = new HashCode();
        Add(hash, () => value);
        return value;
    }
}
