using HomeInventory.Domain;
using HomeInventory.Domain.Primitives;
using HomeInventory.Tests.Framework.Attributes;
using HomeInventory.Tests.Framework.Customizations;

namespace HomeInventory.Tests.Framework;

[InvariantCulture]
[TestCaseOrderer("HomeInventory.Tests.Framework.PriorityTestOrderer", "HomeInventory.Tests.Framework")]
public abstract class BaseTest : CompositeDisposable, IAsyncLifetime
{
    private readonly Lazy<CancellationImplementation> _lazyCancellation = new(() => new CancellationImplementation());
    private readonly Lazy<IFixture> _lazyFixture = new(() => new Fixture());
    private readonly Lazy<IDateTimeService> _lazyDateTime = new(() => new FixedDateTimeService(DateTimeOffset.UtcNow));

    protected BaseTest()
    {
        AddDisposable(_lazyCancellation);
        Fixture.CustomizeUlid();
    }

    protected internal IFixture Fixture => _lazyFixture.Value;

    protected internal ICancellation Cancellation => _lazyCancellation.Value;

    protected internal IDateTimeService DateTime => _lazyDateTime.Value;

    public Task InitializeAsync() => Task.CompletedTask;

    async Task IAsyncLifetime.DisposeAsync() => await DisposeAsync();
}

public abstract class BaseTest<TGiven> : BaseTest
    where TGiven : GivenContext<TGiven>
{
    protected BaseTest(Func<BaseTest, TGiven> createGiven)
    {
        Given = createGiven(this);
        When = CreateWhen(Given.Variables);
    }

    public TGiven Given { get; }

    public WhenContext When { get; }

    private WhenContext CreateWhen(VariablesContainer variables) =>
        new(variables, Cancellation);
}
