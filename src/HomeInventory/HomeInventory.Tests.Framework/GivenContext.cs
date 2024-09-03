using System.Runtime.CompilerServices;

namespace HomeInventory.Tests.Framework;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "False positive")]
public class GivenContext<TContext>(BaseTest test) : BaseContext(new VariablesContainer())
    where TContext : GivenContext<TContext>
{
    private readonly IFixture _fixture = test.Fixture;

    protected TContext This => (TContext)this;

    protected TContext Customize<TCustomization>()
        where TCustomization : ICustomization, new() =>
        Customize(new TCustomization());

    protected TContext Customize(ICustomization customization)
    {
        _fixture.Customize(customization);
        return This;
    }

    public TContext New<T>(out IVariable<T> variable, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull =>
        New(out variable, () => CreateMany<T>(count), name);

    public TContext New<T>(out IVariable<T> variable, Func<T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull =>
        New(out variable, _ => create(), count, name);

    public TContext New<T>(out IVariable<T> variable, Func<int, T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull =>
        New(out variable, () => Enumerable.Range(0, count).Select(create), name);

    public TContext EmptyHashCode(out IVariable<HashCode> emptyHash) =>
        New(out emptyHash, () => new HashCode());

    public TContext SubstituteFor<T, TArg1, TArg2>(out IVariable<T> variable, IVariable<TArg1> arg1, IVariable<TArg2> arg2, Action<T, TArg1, TArg2> setup, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : class
        where TArg1 : notnull
        where TArg2 : notnull =>
        SubstituteFor(out variable, arg1, (value, arg) => setup(value, arg, GetValue(arg2)), name: name);

    public TContext SubstituteFor<T, TArg>(out IVariable<T> variable, IVariable<TArg> argV, Action<T, TArg> setup, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : class
        where TArg : notnull =>
        SubstituteFor(out variable, value => setup(value, GetValue(argV)), name: name);

    public TContext SubstituteFor<T>(out IVariable<T> variable, Action<T> setup, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : class
    {
        return New(out variable, () => Create(setup), name: name);

        static T Create(Action<T> setup)
        {
            var value = Substitute.For<T>();
            setup(value);
            return value;
        }
    }

    public TContext AddAllToHashCode<T>(out IVariable<HashCode> hash, IVariable<T> variable, [CallerArgumentExpression(nameof(hash))] string? name = null)
        where T : notnull
    {
        name ??= nameof(AddAllToHashCode);
        hash = new Variable<HashCode>(name);
        var hashValue = Variables.TryGetOrAdd(hash[0], () => new HashCode())
            .ThrowIfNone(() => new InvalidOperationException($"Failed to add variable '{name}' of type {typeof(HashCode)}"))
            .Value;

        foreach (var value in Variables.GetMany(variable))
        {
            hashValue.Add(value);
        }

        Variables.TryUpdate(hash[0], () => hashValue)
            .ThrowIfNone(() => new InvalidOperationException($"Failed to update variable '{name}' of type {typeof(HashCode)}"));

        return This;
    }

    protected TContext Add<T>(IVariable<T> variable, Func<IEnumerable<T>> createValues)
        where T : notnull
    {
        foreach (var value in createValues())
        {
            Variables.Add(variable, () => value);
        }

        return This;
    }

    protected T Create<T>() => _fixture.Create<T>();

    protected IEnumerable<T> CreateMany<T>() => _fixture.CreateMany<T>();

    protected IEnumerable<T> CreateMany<T>(int count) => _fixture.CreateMany<T>(count);

    private TContext New<T>(out IVariable<T> variable, Func<IEnumerable<T>> createMany, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull
    {
        variable = new Variable<T>(name ?? typeof(T).Name);

        return Add(variable, createMany);
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

public abstract class GivenContext<TGiven, TSut, TArg1, TArg2>(BaseTest test) : GivenContext<TGiven>(test)
    where TGiven : GivenContext<TGiven, TSut, TArg1, TArg2>
    where TSut : notnull
    where TArg1 : notnull
    where TArg2 : notnull
{
    public TGiven Sut(out IVariable<TSut> sut, IVariable<TArg1> arg1, IVariable<TArg2> arg2, int count = 1, [CallerArgumentExpression(nameof(sut))] string? name = null) =>
        New(out sut, i => CreateSut(GetValue(arg1[i]), GetValue(arg2[i])), count, name);

    public TGiven Sut(out IVariable<TSut> sut, IIndexedVariable<TArg1> arg1 , IIndexedVariable<TArg2> arg2, [CallerArgumentExpression(nameof(sut))] string? name = null) =>
        New(out sut, () => CreateSut(GetValue(arg1), GetValue(arg2)), name: name);

    protected abstract TSut CreateSut(TArg1 arg1, TArg2 arg2);
}
