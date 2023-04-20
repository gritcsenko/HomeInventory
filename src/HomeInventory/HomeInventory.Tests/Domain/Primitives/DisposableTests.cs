using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Domain.Primitives;

[Trait("Category", "Unit")]
public class DisposableTests : BaseTest
{
    [Fact]
    public void Dispose_Should_InvokeAction()
    {
        var called = false;
        void Action() => called = true;
        var sut = Disposable.Create(Action);

        sut.Dispose();

        called.Should().BeTrue();
    }

    [Fact]
    public void Dispose_Should_NotInvokeAction_WhenCalledSecondTime()
    {
        var called = false;
        void Action() => called = true;
        var sut = Disposable.Create(Action);
        sut.Dispose();
        called = false;

        sut.Dispose();

        called.Should().BeFalse();
    }

    [Fact]
    public void Dispose_Should_SetIsDisposed()
    {
        void Action()
        { };
        var sut = Disposable.Create(Action);

        sut.Dispose();

        sut.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void Dispose_Should_HaveIsDisposedEqualToFalse_WhenCreated()
    {
        void Action()
        { };

        var sut = Disposable.Create(Action);

        sut.IsDisposed.Should().BeFalse();
    }

    [Fact]
    public void DisposeOfT_Should_InvokeAction()
    {
        var called = false;
        void Action(object state) => called = true;
        var sut = Disposable.Create(Action, new object());

        sut.Dispose();

        called.Should().BeTrue();
    }

    [Fact]
    public void DisposeOfT_Should_NotInvokeAction_WhenCalledSecondTime()
    {
        var called = false;
        void Action(object state) => called = true;
        var sut = Disposable.Create(Action, new object());
        sut.Dispose();
        called = false;

        sut.Dispose();

        called.Should().BeFalse();
    }

    [Fact]
    public void DisposeOfT_Should_PassStateToAction()
    {
        var actualState = default(object?);
        var expectedState = new object();
        void Action(object state) => actualState = state;
        var sut = Disposable.Create(Action, expectedState);

        sut.Dispose();

        actualState.Should().BeSameAs(expectedState);
    }

    [Fact]
    public void DisposeOfT_Should_SetIsDisposed()
    {
        void Action(object _)
        { };
        var sut = Disposable.Create(Action, new object());

        sut.Dispose();

        sut.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void DisposeOfT_Should_HaveIsDisposedEqualToFalse_WhenCreated()
    {
        void Action(object _)
        { };

        var sut = Disposable.Create(Action, new object());

        sut.IsDisposed.Should().BeFalse();
    }
}
