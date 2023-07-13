using HomeInventory.Domain;
using HomeInventory.Domain.Primitives;
using HomeInventory.Tests.Framework.Customizations;

namespace HomeInventory.Tests.Framework;

public abstract class BaseTest : CompositeDisposable
{
    private readonly Lazy<CancellationImplementation> _lazyCancellation = new(() => new CancellationImplementation());
    private readonly Lazy<IFixture> _lazyFixture = new(() => new Fixture());
    private readonly Lazy<IDateTimeService> _lazyDateTime = new(() => new FixedDateTimeService(DateTimeOffset.UtcNow));

    protected BaseTest()
    {
        AddDisposable(_lazyCancellation);
        Fixture.CustomizeUlid();
    }

    protected IFixture Fixture => _lazyFixture.Value;

    protected ICancellation Cancellation => _lazyCancellation.Value;

    protected IDateTimeService DateTime => _lazyDateTime.Value;

    private sealed class CancellationImplementation : CompositeDisposable, ICancellation
    {
        private readonly CancellationTokenSource _source;

        public CancellationImplementation(CancellationTokenSource? source = null)
        {
            _source = source ?? new CancellationTokenSource();
            AddDisposable(_source);
        }

        public CancellationToken Token => _source.Token;

        public void Cancel() => _source.Cancel();
    }
}

public abstract class BaseTest<TGiven> : BaseTest
    where TGiven : GivenContext<TGiven>
{
    private readonly VariablesContainer _variables = new();
    private TGiven? _given;

    protected BaseTest()
    {
        When = CreateWhen(_variables);
    }

    public TGiven Given => _given ??= CreateGiven(_variables);

    public WhenContext When { get; }

    protected IVariable Result { get; } = new Variable(nameof(Result));

    protected abstract TGiven CreateGiven(VariablesContainer variables);

    private WhenContext CreateWhen(VariablesContainer variables) =>
        new(variables, Result, Cancellation);
}
