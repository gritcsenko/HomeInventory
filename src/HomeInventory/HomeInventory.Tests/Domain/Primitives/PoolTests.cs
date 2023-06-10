using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Domain.Primitives;

[Trait("Category", "Unit")]
public class PoolTests : BaseTest
{
    private readonly IPoolObjectActivator<object> _activator = Substitute.For<IPoolObjectActivator<object>>();

    public PoolTests()
    {
        _activator.Pull().Returns(ci => Fixture.Create<object>());
    }

    [Fact]
    public void Count_ShouldBeZero_AfterCreation()
    {
        var sut = CreateSut();

        sut.Count.Should().Be(0);
    }

    [Fact]
    public void Count_ShouldBeZero_AfterCreationWithDefault()
    {
        var sut = CreateSutWithDefault();

        sut.Count.Should().Be(0);
    }

    [Fact]
    public void Count_ShouldBeExpected_AfterFill()
    {
        var expected = Fixture.Create<int>();
        var sut = CreateSut();

        sut.Fill(expected);

        sut.Count.Should().Be(expected);
    }

    [Fact]
    public void Count_ShouldBeExpected_AfterFillWithDefault()
    {
        var expected = Fixture.Create<int>();
        var sut = CreateSutWithDefault();

        sut.Fill(expected);

        sut.Count.Should().Be(expected);
    }

    [Fact]
    public void Count_ShouldBeZero_AfterFillAndClear()
    {
        var sut = CreateSut();
        sut.Fill(Fixture.Create<int>());

        sut.Clear();

        sut.Count.Should().Be(0);
    }

    [Fact]
    public void Pull_ShouldReturnExpected_AfterCreation()
    {
        var expected = Fixture.Create<object>();
        _activator.Pull().Returns(ci => expected);
        var sut = CreateSut();

        var actual = sut.Pull();

        actual.Should().Be(expected);
    }

    [Fact]
    public void Pull_ShouldReturnExpected_AfterOnePush()
    {
        _activator.Pull().Returns(ci => Fixture.Create<object>(), ci => throw new InvalidOperationException("Not expected call"));
        var sut = CreateSut();
        var expected = sut.Pull();
        sut.Push(expected);

        var actual = sut.Pull();

        actual.Should().Be(expected);
    }

    [Fact]
    public void Pull_ShouldNotChangeCount_AfterCall()
    {
        var sut = CreateSut();
        var expected = sut.Count;

        _ = sut.Pull();

        sut.Count.Should().Be(expected);
    }

    [Fact]
    public void Push_ShouldIncreaseCount_AfterCall()
    {
        var sut = CreateSut();
        var expected = sut.Count + 1;

        sut.Push(Fixture.Create<object>());

        sut.Count.Should().Be(expected);
    }

    [Fact]
    public void Push_ShouldCallActivatorsPush_DuringCall()
    {
        var sut = CreateSut();
        var expected = Fixture.Create<object>();
        var pushCalled = false;
        object? pushArg = null;
        _activator
            .When(a => a.Push(Arg.Any<object>()))
            .Do(ci =>
            {
                pushCalled = true;
                pushArg = ci.Arg<object>();
            });

        sut.Push(expected);

        pushCalled.Should().BeTrue();
        pushArg.Should().Be(expected);
    }

    private Pool<object> CreateSut() => new(_activator);

    private static Pool<object> CreateSutWithDefault() => new();
}
