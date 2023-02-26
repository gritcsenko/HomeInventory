using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Domain.Primitives;

[Trait("Category", "Unit")]
public class AsyncDisposableTests : BaseTest
{
    [Fact]
    public async ValueTask Dispose_Should_InvokeAction()
    {
        var called = false;
        ValueTask Action()
        {
            called = true;
            return ValueTask.CompletedTask;
        }

        var sut = AsyncDisposable.Create(Action);

        await sut.DisposeAsync();

        called.Should().BeTrue();
    }

    [Fact]
    public async ValueTask Dispose_Should_NotInvokeAction_WhenCalledSecondTime()
    {
        var called = false;
        ValueTask Action()
        {
            called = true;
            return ValueTask.CompletedTask;
        }
        var sut = AsyncDisposable.Create(Action);
        await sut.DisposeAsync();
        called = false;

        await sut.DisposeAsync();

        called.Should().BeFalse();
    }

    [Fact]
    public async ValueTask Dispose_Should_SetIsDisposed()
    {
        ValueTask Action() => ValueTask.CompletedTask;
        var sut = AsyncDisposable.Create(Action);

        await sut.DisposeAsync();

        sut.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void Dispose_Should_HaveIsDisposedEqualToFalse_WhenCreated()
    {
        ValueTask Action() => ValueTask.CompletedTask;

        var sut = AsyncDisposable.Create(Action);

        sut.IsDisposed.Should().BeFalse();
    }

    [Fact]
    public async ValueTask DisposeOfT_Should_InvokeAction()
    {
        var called = false;
        ValueTask Action(object state)
        {
            called = true;
            return ValueTask.CompletedTask;
        }
        var sut = AsyncDisposable.Create(Action, new object());

        await sut.DisposeAsync();

        called.Should().BeTrue();
    }

    [Fact]
    public async ValueTask DisposeOfT_Should_NotInvokeAction_WhenCalledSecondTime()
    {
        var called = false;
        ValueTask Action(object state)
        {
            called = true;
            return ValueTask.CompletedTask;
        }
        var sut = AsyncDisposable.Create(Action, new object());
        await sut.DisposeAsync();
        called = false;

        await sut.DisposeAsync();

        called.Should().BeFalse();
    }

    [Fact]
    public async ValueTask DisposeOfT_Should_PassStateToAction()
    {
        var actualState = default(object?);
        var expectedState = new object();
        ValueTask Action(object state)
        {
            actualState = state;
            return ValueTask.CompletedTask;
        }
        var sut = AsyncDisposable.Create(Action, expectedState);

        await sut.DisposeAsync();

        actualState.Should().BeSameAs(expectedState);
    }

    [Fact]
    public async ValueTask DisposeOfT_Should_SetIsDisposed()
    {
        ValueTask Action(object _) => ValueTask.CompletedTask;
        var sut = AsyncDisposable.Create(Action, new object());

        await sut.DisposeAsync();

        sut.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void DisposeOfT_Should_HaveIsDisposedEqualToFalse_WhenCreated()
    {
        ValueTask Action(object _) => ValueTask.CompletedTask;

        var sut = AsyncDisposable.Create(Action, new object());

        sut.IsDisposed.Should().BeFalse();
    }
}
