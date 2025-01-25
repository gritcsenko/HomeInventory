using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace HomeInventory.Tests.Framework;

[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "False positive")]
public class GivenContext<TContext>(BaseTest test) : BaseContext(new())
    where TContext : GivenContext<TContext>
{
    private readonly IFixture _fixture = test.Fixture;

    protected TContext This => (TContext)this;

    protected TContext Customize<TCustomization>()
        where TCustomization : ICustomization, new() =>
        Customize(new TCustomization());

    public TContext New<T>(out IVariable<T> variable, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull =>
        New(out variable, () => CreateMany<T>(count), name);

    public TContext New<T>(out IVariable<T> variable, Func<T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull =>
        New(out variable, _ => create(), count, name);

    public TContext New<T, TArg>(out IVariable<T> variable, IVariable<TArg> arg, Func<TArg, T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull
        where TArg : notnull =>
        New(out variable, _ => create(GetValue(arg)), count, name);

    public TContext New<T, TArg1, TArg2>(out IVariable<T> variable, IVariable<TArg1> arg1, IVariable<TArg2> arg2, Func<TArg1, TArg2, T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull
        where TArg1 : notnull
        where TArg2 : notnull =>
        New(out variable, _ => create(GetValue(arg1), GetValue(arg2)), count, name);

    public TContext New<T, TArg1, TArg2, TArg3>(out IVariable<T> variable, IVariable<TArg1> arg1, IVariable<TArg2> arg2, IVariable<TArg3> arg3, Func<TArg1, TArg2, TArg3, T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull
        where TArg1 : notnull
        where TArg2 : notnull
        where TArg3 : notnull =>
        New(out variable, _ => create(GetValue(arg1), GetValue(arg2), GetValue(arg3)), count, name);

    public TContext EmptyHashCode(out IVariable<HashCode> emptyHash) =>
        New(out emptyHash, static () => new());

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
        return New(out variable, () => CreateAndSetup(setup), name: name);

        static T CreateAndSetup(Action<T> setup)
        {
            var value = Substitute.For<T>();
            setup(value);
            return value;
        }
    }

    public TContext SubstituteFor<T>(out IVariable<T> variable, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : class =>
        New(out variable, static () => Substitute.For<T>(), name: name);

    public TContext AddAllToHashCode<T>(out IVariable<HashCode> hash, IVariable<T> variable, [CallerArgumentExpression(nameof(hash))] string? name = null)
        where T : notnull
    {
        name ??= nameof(AddAllToHashCode);
        hash = new Variable<HashCode>(name);
        var hashValue = Variables.TryGetOrAdd(hash[0], () => new())
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

    protected T Create<T>() => _fixture.Create<T>();

    [SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "False positive")]
    protected TContext New<T>(out IVariable<T> variable, Func<int, T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull =>
        New(out variable, () => Enumerable.Range(0, count).Select(create), name);

    private TContext Customize(ICustomization customization)
    {
        _fixture.Customize(customization);
        return This;
    }

    private TContext Add<T>(IVariable<T> variable, Func<IEnumerable<T>> createValues)
        where T : notnull
    {
        foreach (var value in createValues())
        {
            Variables.Add(variable, () => value);
        }

        return This;
    }

    private IEnumerable<T> CreateMany<T>(int count) => _fixture.CreateMany<T>(count);

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
