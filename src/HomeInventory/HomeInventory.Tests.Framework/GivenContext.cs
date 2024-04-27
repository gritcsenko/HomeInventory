namespace HomeInventory.Tests.Framework;

public class GivenContext<TContext>(BaseTest test) : BaseContext(new VariablesContainer())
    where TContext : GivenContext<TContext>
{
    private readonly IFixture _fixture = test.Fixture;

    protected TContext This => (TContext)this;

    public TContext Customize(ICustomization customization)
    {
        _fixture.Customize(customization);
        return This;
    }

    public TContext New<T>(IVariable<T> variable, int count = 1)
        where T : notnull
    {
        foreach (var item in _fixture.CreateMany<T>(count))
        {
            Add(variable, item);
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

    public TContext SubstituteFor<T, TArg>(IVariable<T> variable, IVariable<TArg> argVariable, params Action<T, TArg>[] setups)
        where T : class
        where TArg : notnull =>
        Add(variable, () =>
        {
            var value = Substitute.For<T>();
            var arg = GetValue(argVariable);
            foreach (var setup in setups)
            {
                setup(value, arg);
            }
            return value;
        });

    public TContext SubstituteFor<T, TArg1, TArg2>(IVariable<T> variable, IVariable<TArg1> arg1Variable, IVariable<TArg2> arg2Variable, params Action<T, TArg1, TArg2>[] setups)
        where T : class
        where TArg1 : notnull
        where TArg2 : notnull =>
        Add(variable, () =>
        {
            var value = Substitute.For<T>();
            var arg1 = GetValue(arg1Variable);
            var arg2 = GetValue(arg2Variable);
            foreach (var setup in setups)
            {
                setup(value, arg1, arg2);
            }
            return value;
        });

    public TContext SubstituteFor<T>(IVariable<T> variable, IVariable<int> countVariable, params Action<T, int, VariablesContainer>[] setups)
        where T : class
    {
        foreach (var index in Enumerable.Range(0, GetValue(countVariable)))
        {
            Add(variable, () =>
            {
                var value = Substitute.For<T>();
                foreach (var setup in setups)
                {
                    setup(value, index, Variables);
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
        where T : notnull
    {
        foreach (var _ in Enumerable.Range(0, GetValue(countVariable)))
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

    protected T Create<T>() => _fixture.Create<T>();

    private HashCode AddNewHashCode(IIndexedVariable<HashCode> hash)
    {
        var value = new HashCode();
        Add(hash, () => value);
        return value;
    }
}
public abstract class GivenContext<TContext, TSut>(BaseTest test) : GivenContext<TContext>(test)
    where TContext : GivenContext<TContext, TSut>
    where TSut : notnull
{
    private readonly Variable<TSut> _sut = new(nameof(_sut));

    internal protected IVariable<TSut> Sut => _sut;

    internal protected TContext AddSut() => Add(_sut, CreateSut);

    protected abstract TSut CreateSut();
}