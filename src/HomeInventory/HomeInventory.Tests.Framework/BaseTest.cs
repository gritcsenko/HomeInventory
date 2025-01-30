using System.Diagnostics.CodeAnalysis;
using HomeInventory.Tests.Framework.Attributes;

namespace HomeInventory.Tests.Framework;

[InvariantCulture]
[TestCaseOrderer("HomeInventory.Tests.Framework.PriorityTestOrderer", "HomeInventory.Tests.Framework")]
public abstract class BaseTest : IAsyncLifetime
{
    private readonly List<IAsyncDisposable> _asyncDisposables = [];
    private readonly Lazy<CancellationImplementation> _lazyCancellation = new(static () => new());
    private readonly Lazy<IFixture> _lazyFixture = new(static () => new Fixture());
    private readonly Lazy<TimeProvider> _lazyDateTime = new(static () => new FixedTimeProvider(TimeProvider.System));

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "AddDisposable always disposes the object later.")]
    protected BaseTest() => AddDisposable(_lazyCancellation.ToDisposable());

    protected internal IFixture Fixture => _lazyFixture.Value;

    protected internal ICancellation Cancellation => _lazyCancellation.Value;

    protected TimeProvider DateTime => _lazyDateTime.Value;

    public virtual Task InitializeAsync() => Task.CompletedTask;

    public virtual async Task DisposeAsync()
    {
        foreach (var disposable in _asyncDisposables)
        {
            await disposable.DisposeAsync();
        }
    }

    protected void AddDisposable(IDisposable disposable) => AddAsyncDisposable(disposable.ToAsyncDisposable());
    
    protected void AddAsyncDisposable(IAsyncDisposable disposable) => _asyncDisposables.Add(disposable);
}

public abstract class BaseTest<TGiven> : BaseTest
    where TGiven : GivenContext<TGiven>
{
    private readonly Lazy<TGiven> _lazyGiven;
    private readonly Lazy<WhenContext> _lazyWhen;

    protected BaseTest(Func<BaseTest<TGiven>, TGiven> createGiven)
    {
        _lazyGiven = new(() => createGiven(this));
        _lazyWhen = new(() => new(Given.Variables, Cancellation));
    }

    protected TGiven Given => _lazyGiven.Value;

    protected WhenContext When => _lazyWhen.Value;
}
