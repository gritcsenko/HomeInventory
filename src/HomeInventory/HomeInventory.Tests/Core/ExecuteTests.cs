namespace HomeInventory.Tests.Core;

[UnitTest]
public class ExecuteTests
{
    [Fact]
    public async Task AndCatchAsync_ShouldExecuteAction()
    {
        var isExecuted = false;
        Task AsyncAction()
        {
            isExecuted = true;
            return Task.CompletedTask;
        }

        await Execute.AndCatchAsync(AsyncAction, (Exception _) => false.Should().BeTrue());

        isExecuted.Should().BeTrue();
    }

    [Fact]
    public async Task AndCatchAsync_ShouldCatchException()
    {
        var expected = new InvalidOperationException();
        Exception? actual = null;
        Task AsyncAction() => throw expected;

        await Execute.AndCatchAsync(AsyncAction, (Exception ex) => actual = ex);

        actual.Should().Be(expected);
    }

    [Fact]
    public async Task AndCatchAsync_ShouldReturnResult()
    {
        var expected = Guid.NewGuid();
        Task<Guid> AsyncAction() => Task.FromResult(expected);

        var actual = await Execute.AndCatchAsync(AsyncAction, (Exception _) =>
        {
            false.Should().BeTrue();
            return Guid.Empty;
        });

        actual.Should().Be(expected);
    }

    [Fact]
    public async Task AndCatchAsync_ShouldCatchExceptionAndReturnResult()
    {
        var expected = new InvalidOperationException();
        var expectedResult = Guid.NewGuid();
        Exception? actual = null;
        Task<Guid> AsyncAction() => throw expected;

        var actualResult = await Execute.AndCatchAsync(AsyncAction, (Exception ex) =>
        {
            actual = ex;
            return expectedResult;
        });

        actual.Should().Be(expected);
        actualResult.Should().Be(expectedResult);
    }

    [Fact]
    public void AndCatch_ShouldReturnResult()
    {
        var expected = Guid.NewGuid();

        var actual = Execute.AndCatch(() => expected, (Exception _) =>
        {
            false.Should().BeTrue();
            return Guid.Empty;
        });

        actual.Should().Be(expected);
    }

    [Fact]
    public void AndCatch_ShouldCatchExceptionAndReturnResult()
    {
        var expected = new InvalidOperationException();
        var expectedResult = Guid.NewGuid();
        Exception? actual = null;
        Guid AsyncAction() => throw expected;

        var actualResult = Execute.AndCatch(AsyncAction, (Exception ex) =>
        {
            actual = ex;
            return expectedResult;
        });

        actual.Should().Be(expected);
        actualResult.Should().Be(expectedResult);
    }
}
