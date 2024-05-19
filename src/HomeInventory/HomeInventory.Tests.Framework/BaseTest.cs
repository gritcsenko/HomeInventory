using HomeInventory.Core;
using HomeInventory.Domain;
using HomeInventory.Domain.Primitives;
using HomeInventory.Tests.Framework.Attributes;
using HomeInventory.Tests.Framework.Customizations;
using System.Reactive.Disposables;

namespace HomeInventory.Tests.Framework;

[InvariantCulture]
[TestCaseOrderer("HomeInventory.Tests.Framework.PriorityTestOrderer", "HomeInventory.Tests.Framework")]
public abstract class BaseTest : IAsyncLifetime
{
    private readonly CompositeDisposable _disposables = [];
    private readonly Lazy<CancellationImplementation> _lazyCancellation = new(() => new CancellationImplementation());
    private readonly Lazy<IFixture> _lazyFixture = new(() => new Fixture());
    private readonly Lazy<IDateTimeService> _lazyDateTime = new(() => new FixedDateTimeService(DateTimeOffset.UtcNow));

    protected BaseTest()
    {
    }

    protected internal IFixture Fixture => _lazyFixture.Value;

    protected internal ICancellation Cancellation => _lazyCancellation.Value;

    protected internal IDateTimeService DateTime => _lazyDateTime.Value;

    public virtual Task InitializeAsync()
    {
        Fixture.CustomizeUlid();
        _disposables.AddAll(InitializeDisposables());
        return Task.CompletedTask;
    }

    public virtual Task DisposeAsync()
    {
        _disposables.Dispose();
        return Task.CompletedTask;
    }

    protected virtual IEnumerable<IDisposable> InitializeDisposables()
    {
        yield return _lazyCancellation.ToDisposable();
    }
}

public abstract class BaseTest<TGiven> : BaseTest
    where TGiven : GivenContext<TGiven>
{
    private readonly Lazy<TGiven> _lazyGiven;
    private readonly Lazy<WhenContext> _lazyWhen;

    protected BaseTest(Func<BaseTest, TGiven> createGiven)
    {
        _lazyGiven = new(() => createGiven(this));
        _lazyWhen = new(() => new WhenContext(Given.Variables, Cancellation));
    }

    protected TGiven Given => _lazyGiven.Value;

    protected WhenContext When => _lazyWhen.Value;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Given.Initialize();
    }
}
