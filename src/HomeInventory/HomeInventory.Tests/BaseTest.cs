using System.Runtime.Serialization;

namespace HomeInventory.Tests;

public abstract class BaseTest : Disposable
{
    private readonly Lazy<CancellationImplementation> _lazyCancellation = new(() => new CancellationImplementation());
    private readonly Lazy<IFixture> _lazyFixture = new(() => new Fixture());

    protected BaseTest()
    {
    }

    protected IFixture Fixture => _lazyFixture.Value;

    protected ICancellation Cancellation => _lazyCancellation.Value;

    protected static object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;

        // Type without parameterless constructor
        return FormatterServices.GetUninitializedObject(type);
    }

    protected override void InternalDispose()
    {
        if (_lazyCancellation.IsValueCreated)
        {
            _lazyCancellation.Value.Dispose();
        }
    }

    protected interface ICancellation
    {
        CancellationToken Token { get; }

        void Cancel();
    }

    private sealed class CancellationImplementation : Disposable, ICancellation
    {
        private readonly CancellationTokenSource _source;

        public CancellationImplementation(CancellationTokenSource? source = null) => _source = source ?? new CancellationTokenSource();

        public CancellationToken Token => _source.Token;

        public void Cancel() => _source.Cancel();

        protected override void InternalDispose() => _source.Dispose();
    }
}

public abstract class BaseTest<TGiven, TWhen, TThen> : BaseTest
    where TGiven : GivenContext<TGiven>
    where TWhen : WhenContext
    where TThen : ThenContext
{
    private readonly VariablesCollection _variables = new();

    protected BaseTest()
    {
        Given = CreateGiven(_variables);
        When = CreateWhen(_variables);
        Then = CreateThen(_variables);
    }

    public TGiven Given { get; }

    public TWhen When { get; }

    public TThen Then { get; }

    protected IVariable Result { get; } = new Variable(nameof(Result));

    protected abstract TGiven CreateGiven(VariablesCollection variables);

    protected abstract TWhen CreateWhen(VariablesCollection variables);

    protected abstract TThen CreateThen(VariablesCollection variables);
}
