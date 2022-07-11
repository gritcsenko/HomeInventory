using AutoFixture;

namespace HomeInventory.Tests.Systems.Controllers;

public abstract class BaseTest : IDisposable
{
    private readonly Fixture _fixture;
    private readonly CancellationTokenSource _source = new CancellationTokenSource();

    public BaseTest()
    {
        _fixture = new Fixture();
    }

    protected IFixture Fixture => _fixture;

    protected CancellationToken CancellationToken => _source.Token;

    void IDisposable.Dispose()
    {
        _source.Dispose();
    }

}
