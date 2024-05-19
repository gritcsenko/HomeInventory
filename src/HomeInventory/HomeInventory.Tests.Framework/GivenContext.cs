using System.Reflection;
using System.Runtime.CompilerServices;

namespace HomeInventory.Tests.Framework;

public class GivenContext<TContext>(BaseTest test) : BaseContext(new VariablesContainer())
    where TContext : GivenContext<TContext>
{
    private readonly IFixture _fixture = test.Fixture;

    protected TContext This => (TContext)this;

    internal virtual void Initialize()
    {
    }

    public TContext Customize(ICustomization customization)
    {
        _fixture.Customize(customization);
        return This;
    }

    public TContext New<T>(out IVariable<T> variable, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull =>
        New(out variable, () => _fixture.CreateMany<T>(count), name);

    public TContext New<T>(out IVariable<T> variable, Func<T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull =>
        New(out variable, _ => create(), count, name);

    public TContext New<T>(out IVariable<T> variable, Func<int, T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull =>
        New(out variable, () => Enumerable.Range(0, count).Select(create), name);

    public TContext New<T>(out IVariable<T> variable, Func<IEnumerable<T>> createMany, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull
    {
        variable = new Variable<T>(name ?? typeof(T).Name);

        foreach (var item in createMany())
        {
            Add(variable, item);
        }

        return This;
    }

    public TContext EmptyHashCode(out IVariable<HashCode> hash)
    {
        hash = new Variable<HashCode>(nameof(EmptyHashCode));
        return Add(hash, () => new HashCode());
    }

    public TContext SubstituteFor<T>(out IVariable<T> variable, params Action<T, VariablesContainer>[] setups)
        where T : class
    {
        variable = new Variable<T>(typeof(T).Name);
        return Add(variable, () =>
        {
            var value = Substitute.For<T>();
            foreach (var setup in setups)
            {
                setup(value, Variables);
            }
            return value;
        });
    }

    public TContext SubstituteFor<T, TArg>(out IVariable<T> variable, IVariable<TArg> argVariable, params Action<T, TArg>[] setups)
        where T : class
        where TArg : notnull
    {
        variable = new Variable<T>(typeof(T).Name);
        return Add(variable, () =>
        {
            var value = Substitute.For<T>();
            var arg = GetValue(argVariable);
            foreach (var setup in setups)
            {
                setup(value, arg);
            }
            return value;
        });
    }

    public TContext SubstituteFor<T, TArg1, TArg2>(out IVariable<T> variable, IVariable<TArg1> arg1Variable, IVariable<TArg2> arg2Variable, params Action<T, TArg1, TArg2>[] setups)
        where T : class
        where TArg1 : notnull
        where TArg2 : notnull
    {
        variable = new Variable<T>(typeof(T).Name);
        return Add(variable, () =>
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
    }

    public TContext SubstituteFor<T>(out IVariable<T> variable, IVariable<int> countVariable, params Action<T, int, VariablesContainer>[] setups)
        where T : class
    {
        variable = new Variable<T>(typeof(T).Name);

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

    public TContext AddAllToHashCode<T>(out IVariable<HashCode> hash, IVariable<T> variable)
        where T : notnull
    {
        hash = new Variable<HashCode>(nameof(AddAllToHashCode));
        var indexed = hash[0];
        var hashValue = Variables.TryGet(indexed)
            .OrInvoke(() => AddNewHashCode(indexed));

        foreach (var value in Variables.GetMany(variable))
        {
            hashValue.Add(value);
        }

        return Variables.TryUpdate(indexed, () => hashValue)
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

public abstract class GivenContext<TGiven, TSut>(BaseTest test) : GivenContext<TGiven>(test)
    where TGiven : GivenContext<TGiven, TSut>
    where TSut : notnull
{
    public TGiven Sut(out IVariable<TSut> sut, int count = 1, [CallerArgumentExpression(nameof(sut))] string? name = null) =>
        New(out sut, CreateSut, count, name);

    protected abstract TSut CreateSut();
}

public abstract class GivenContext<TGiven, TSut, TArg>(BaseTest test) : GivenContext<TGiven>(test)
    where TGiven : GivenContext<TGiven, TSut, TArg>
    where TSut : notnull
    where TArg : notnull
{
    public TGiven Sut(out IVariable<TSut> sut, IVariable<TArg> arg, int count = 1, [CallerArgumentExpression(nameof(sut))] string? name = null) =>
        New(out sut, i => CreateSut(GetValue(arg[i])), count, name);

    public TGiven Sut(out IVariable<TSut> sut, IIndexedVariable<TArg> arg, [CallerArgumentExpression(nameof(sut))] string? name = null) =>
        New(out sut, () => CreateSut(GetValue(arg)), name: name);

    protected abstract TSut CreateSut(TArg arg);
}
