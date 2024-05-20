﻿using HomeInventory.Core;
using HomeInventory.Domain;
using HomeInventory.Domain.Primitives;
using HomeInventory.Tests.Framework.Attributes;
using HomeInventory.Tests.Framework.Customizations;

namespace HomeInventory.Tests.Framework;

[InvariantCulture]
[TestCaseOrderer("HomeInventory.Tests.Framework.PriorityTestOrderer", "HomeInventory.Tests.Framework")]
public abstract class BaseTest : IAsyncLifetime
{
    private readonly List<IAsyncDisposable> _asyncDisposables = [];
    private readonly Lazy<CancellationImplementation> _lazyCancellation = new(() => new CancellationImplementation());
    private readonly Lazy<IFixture> _lazyFixture = new(() => new Fixture());
    private readonly Lazy<IDateTimeService> _lazyDateTime = new(() => new FixedDateTimeService(DateTimeOffset.UtcNow));

    protected BaseTest()
    {
        AddDisposable(_lazyCancellation.ToDisposable());
    }

    protected internal IFixture Fixture => _lazyFixture.Value;

    protected internal ICancellation Cancellation => _lazyCancellation.Value;

    protected internal IDateTimeService DateTime => _lazyDateTime.Value;

    public virtual Task InitializeAsync()
    {
        Fixture.CustomizeUlid();
        return Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        foreach (var disposable in _asyncDisposables)
        {
            await disposable.DisposeAsync();
        }
    }

    protected void AddDisposable(IDisposable disposable) => AddDisposable(disposable.ToAsyncDisposable());

    protected void AddDisposable(IAsyncDisposable disposable) => AddAsyncDisposable(disposable);

    protected void AddAsyncDisposable(IAsyncDisposable disposable) => _asyncDisposables.Add(disposable);
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
        AddDisposable(_lazyGiven.ToAsyncDisposable());
    }

    protected TGiven Given => _lazyGiven.Value;

    protected WhenContext When => _lazyWhen.Value;
}
