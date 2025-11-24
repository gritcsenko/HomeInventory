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

    public TContext Null<T>(out IVariable<T?> variable, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, () => default, count, name);

    public TContext New<T>(out IVariable<T> variable, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, () => CreateMany<T>(count), name);

    public TContext New<T>(out IVariable<T> variable, Func<T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, _ => create(), count, name);

    public TContext New<T, TArg>(out IVariable<T> variable, IVariable<TArg> arg, Func<TArg, T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, _ => create(GetValue(arg)), count, name);

    public TContext New<T, TArg1, TArg2>(out IVariable<T> variable, IVariable<TArg1> arg1, IVariable<TArg2> arg2, Func<TArg1, TArg2, T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, _ => create(GetValue(arg1), GetValue(arg2)), count, name);

    public TContext New<T, TArg1, TArg2, TArg3>(out IVariable<T> variable, IVariable<TArg1> arg1, IVariable<TArg2> arg2, IVariable<TArg3> arg3, Func<TArg1, TArg2, TArg3, T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, _ => create(GetValue(arg1), GetValue(arg2), GetValue(arg3)), count, name);

    public TContext New<T, TArg1, TArg2, TArg3, TArg4>(out IVariable<T> variable, IVariable<TArg1> arg1, IVariable<TArg2> arg2, IVariable<TArg3> arg3, IVariable<TArg4> arg4, Func<TArg1, TArg2, TArg3, TArg4, T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, _ => create(GetValue(arg1), GetValue(arg2), GetValue(arg3), GetValue(arg4)), count, name);

    public TContext EmptyHashCode(out IVariable<HashCode> emptyHash) =>
        New(out emptyHash, static () => new());

    public TContext SubstituteFor<T, TArg1, TArg2>(out IVariable<T> variable, IVariable<TArg1> arg1, IVariable<TArg2> arg2, Action<T, TArg1, TArg2> setup, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : class =>
        SubstituteFor(out variable, arg1, (value, arg) => setup(value, arg, GetValue(arg2)), name: name);

    public TContext SubstituteFor<T, TArg1, TArg2, TArg3>(out IVariable<T> variable, IVariable<TArg1> arg1, IVariable<TArg2> arg2, IVariable<TArg3> arg3, Action<T, TArg1, TArg2, TArg3> setup, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : class =>
        SubstituteFor(out variable, arg1, arg2, (value, a1, a2) => setup(value, a1, a2, GetValue(arg3)), name: name);

    public TContext SubstituteFor<T, TArg>(out IVariable<T> variable, IVariable<TArg> argV, Action<T, TArg> setup, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : class =>
        SubstituteFor(out variable, value => setup(value, GetValue(argV)), name: name);

    public TContext SubstituteFor<T>(out IVariable<T> variable, Action<T> setup, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : class =>
        New(out variable, () =>
        {
            var value = Substitute.For<T>();
            setup(value);
            return value;
        }, name: name);

    public TContext SubstituteFor<T>(out IVariable<T> variable, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : class =>
        New(out variable, static () => Substitute.For<T>(), name: name);

    public TContext AddAllToHashCode<T>(out IVariable<HashCode> hash, IVariable<T> variable, [CallerArgumentExpression(nameof(hash))] string? name = null)
    {
        var hashVar = CreateVariable<HashCode>(name);
        hash = hashVar;
        var indexed = hashVar[0];

        var hashValue = Variables.TryGetOrAdd(indexed, () => new())
            .ThrowIfNone(() => new InvalidOperationException($"Failed to add variable '{indexed.Name}' of type {typeof(HashCode)}"))
            .Value;

        Variables.GetMany(variable).ForEach(v => hashValue.Add(v));

        Variables.TryUpdate(indexed, () => hashValue)
            .ThrowIfNone(() => new InvalidOperationException($"Failed to update variable '{indexed.Name}' of type {typeof(HashCode)}"));

        return This;
    }

    protected T Create<T>() => _fixture.Create<T>();

    protected IEnumerable<T> CreateMany<T>(int count) => _fixture.CreateMany<T>(count);

    [SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "False positive")]
    protected TContext New<T>(out IVariable<T> variable, Func<int, T> create, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, () => Enumerable.Range(0, count).Select(create), name);

    private TContext Customize(ICustomization customization)
    {
        _fixture.Customize(customization);
        return This;
    }

    private TContext Add<T>(IVariable<T> variable, Func<IEnumerable<T>> createValues)
    {
        createValues().ForEach(value => Variables.Add(variable, () => value));

        return This;
    }

    private TContext New<T>(out IVariable<T> variable, Func<IEnumerable<T>> createMany, [CallerArgumentExpression(nameof(variable))] string? name = null)
    {
        variable = CreateVariable<T>(name);
        return Add(variable, createMany);
    }

    private static Variable<T> CreateVariable<T>(string? name) => new(name ?? typeof(T).Name);
}

public abstract class GivenContext<TGiven, TSut>(BaseTest test) : GivenContext<TGiven>(test)
    where TGiven : GivenContext<TGiven, TSut>
{
    public TGiven Sut(out IVariable<TSut> sut, int count = 1, [CallerArgumentExpression(nameof(sut))] string? name = null) =>
        New(out sut, CreateSut, count, name);

    protected abstract TSut CreateSut();
}

public abstract class GivenContext<TGiven, TSut, TArg>(BaseTest test) : GivenContext<TGiven>(test)
    where TGiven : GivenContext<TGiven, TSut, TArg>
{
    public TGiven Sut(out IVariable<TSut> sut, IVariable<TArg> arg, int count = 1, [CallerArgumentExpression(nameof(sut))] string? name = null) =>
        New(out sut, i => CreateSut(GetValue(arg[i])), count, name);

    public TGiven Sut(out IVariable<TSut> sut, IIndexedVariable<TArg> arg, [CallerArgumentExpression(nameof(sut))] string? name = null) =>
        New(out sut, () => CreateSut(GetValue(arg)), name: name);

    protected abstract TSut CreateSut(TArg arg);
}

public abstract class GivenContext<TGiven, TSut, TArg1, TArg2>(BaseTest test) : GivenContext<TGiven>(test)
    where TGiven : GivenContext<TGiven, TSut, TArg1, TArg2>
{
    public TGiven Sut(out IVariable<TSut> sut, IVariable<TArg1> arg1, IVariable<TArg2> arg2, int count = 1, [CallerArgumentExpression(nameof(sut))] string? name = null) =>
        New(out sut, i => CreateSut(GetValue(arg1[i]), GetValue(arg2[i])), count, name);

    public TGiven Sut(out IVariable<TSut> sut, IIndexedVariable<TArg1> arg1, IIndexedVariable<TArg2> arg2, [CallerArgumentExpression(nameof(sut))] string? name = null) =>
        New(out sut, () => CreateSut(GetValue(arg1), GetValue(arg2)), name: name);

    protected abstract TSut CreateSut(TArg1 arg1, TArg2 arg2);
}
