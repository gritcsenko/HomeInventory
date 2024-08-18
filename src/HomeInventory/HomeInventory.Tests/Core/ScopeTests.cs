namespace HomeInventory.Tests.Core;

public sealed class ScopeTests
{
    [Fact]
    public void Get_ShouldReturnNull_WhenNothingIsSet()
    {
        var sut = CreateSut<GetNullContext>();

        var actual = sut.TryGet();

        actual.Should().BeNone();
    }

    [Fact]
    public void Get_ShouldReturnContext_WhenItIsSet()
    {
        var sut = CreateSut<GetNotNullContext>();
        var expected = new GetNotNullContext();

        sut.Set(expected);
        var actual = sut.TryGet();

        actual.Should().Be(expected);
    }

    [Fact]
    public void Get_ShouldReturnNull_WhenResetIsCalled()
    {
        var sut = CreateSut<ResetContext>();
        sut.Set(new ResetContext());

        sut.Reset();
        var actual = sut.TryGet();

        actual.Should().BeNone();
    }

    [Fact]
    public void Get_ShouldReturnNull_WhenSetAndDisposed()
    {
        var sut = CreateSut<SetNullDisposedContext>();
        var token = sut.Set(new SetNullDisposedContext());

        token.Dispose();
        var actual = sut.TryGet();

        actual.Should().BeNone();
    }


    [Fact]
    public void Get_ShouldReturnContext_WhenResetAndDisposed()
    {
        var sut = CreateSut<ResetNullDisposedContext>();
        var expected = new ResetNullDisposedContext();
        sut.Set(expected);
        var token = sut.Reset();

        token.Dispose();
        var actual = sut.TryGet();

        actual.Should().Be(expected);
    }

    [Fact]
    public void Get_ShouldReturnLatestContext_WhenSetMultipleTimes()
    {
        var sut = CreateSut<SetTwiceContext>();
        var decoy = new SetTwiceContext();
        var expected = new SetTwiceContext();

        sut.Set(decoy);
        sut.Set(expected);
        var actual = sut.TryGet();

        actual.Should().Be(expected);
    }

    [Fact]
    public void Get_ShouldReturnContext_WhenSetMultipleTimesAndDisposed()
    {
        var sut = CreateSut<SetTwiceDisposedContext>();
        var decoy = new SetTwiceDisposedContext();
        var expected = new SetTwiceDisposedContext();
        sut.Set(expected);

        var token = sut.Set(decoy);
        token.Dispose();
        var actual = sut.TryGet();

        actual.Should().Be(expected);
    }

    private static IScope<TContext> CreateSut<TContext>()
        where TContext : class
    {
        var collection = new ServiceCollection();
        collection.AddDomain();
        var accessor = collection.BuildServiceProvider().GetRequiredService<IScopeAccessor>();
        return accessor.GetScope<TContext>();
    }

    private sealed class GetNullContext
    {
        public GetNullContext() { }
    };

    private sealed class GetNotNullContext
    {
        public GetNotNullContext() { }
    };

    private sealed class ResetContext
    {
        public ResetContext() { }
    };

    private sealed class SetNullDisposedContext
    {
        public SetNullDisposedContext() { }
    };

    private sealed class ResetNullDisposedContext
    {
        public ResetNullDisposedContext() { }
    };

    private sealed class SetTwiceContext
    {
        public SetTwiceContext() { }
    };

    private sealed class SetTwiceDisposedContext
    {
        public SetTwiceDisposedContext() { }
    };
}
