namespace HomeInventory.Tests.Core;

[UnitTest]
public sealed class ScopeTests
{
    private readonly ScopeAccessor _scopeAccessor = new(new ScopeContainer(new ScopeFactory()));

    [Fact]
    public void Get_ShouldReturnNull_WhenNothingIsSet()
    {
        var sut = _scopeAccessor.GetScope<GetNullContext>();

        var actual = sut.Get();

        actual.Should().BeNone();
    }

    [Fact]
    public void Get_ShouldReturnContext_WhenItIsSet()
    {
        var sut = _scopeAccessor.GetScope<GetNotNullContext>();
        var expected = new GetNotNullContext();

        sut.Set(expected);
        var actual = sut.Get();

        actual.Should().Be(expected);
    }

    [Fact]
    public void Get_ShouldReturnNull_WhenResetIsCalled()
    {
        var sut = _scopeAccessor.GetScope<ResetContext>();
        sut.Set(new());

        sut.Reset();
        var actual = sut.Get();

        actual.Should().BeNone();
    }

    [Fact]
    public void Get_ShouldReturnNull_WhenSetAndDisposed()
    {
        var sut = _scopeAccessor.GetScope<SetNullDisposedContext>();
        var token = sut.Set(new());

        token.Dispose();
        var actual = sut.Get();

        actual.Should().BeNone();
    }


    [Fact]
    public void Get_ShouldReturnContext_WhenResetAndDisposed()
    {
        var sut = _scopeAccessor.GetScope<ResetNullDisposedContext>();
        var expected = new ResetNullDisposedContext();
        sut.Set(expected);
        var token = sut.Reset();

        token.Dispose();
        var actual = sut.Get();

        actual.Should().Be(expected);
    }

    [Fact]
    public void Get_ShouldReturnLatestContext_WhenSetMultipleTimes()
    {
        var sut = _scopeAccessor.GetScope<SetTwiceContext>();
        var decoy = new SetTwiceContext();
        var expected = new SetTwiceContext();

        sut.Set(decoy);
        sut.Set(expected);
        var actual = sut.Get();

        actual.Should().Be(expected);
    }

    [Fact]
    public void Get_ShouldReturnContext_WhenSetMultipleTimesAndDisposed()
    {
        var sut = _scopeAccessor.GetScope<SetTwiceDisposedContext>();
        var decoy = new SetTwiceDisposedContext();
        var expected = new SetTwiceDisposedContext();
        sut.Set(expected);

        var token = sut.Set(decoy);
        token.Dispose();
        var actual = sut.Get();

        actual.Should().Be(expected);
    }

#pragma warning disable S2094
    private sealed class GetNullContext
    {
    };

    private sealed class GetNotNullContext
    {
    };

    private sealed class ResetContext
    {
    };

    private sealed class SetNullDisposedContext
    {
    };

    private sealed class ResetNullDisposedContext
    {
    };

    private sealed class SetTwiceContext
    {
    };

    private sealed class SetTwiceDisposedContext
    {
    };
#pragma warning restore S2094
}
