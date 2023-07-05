namespace HomeInventory.Tests.Core;

[UnitTest]
public class ExecuteTests
{
    [Fact]
    public async Task AndCatchAsync_ShouldExecuteAction()
    {
        var isExecuted = false;
        Task asyncAction()
        {
            isExecuted = true;
            return Task.CompletedTask;
        }

        await Execute.AndCatchAsync(asyncAction, (Exception _) => false.Should().BeTrue());

        isExecuted.Should().BeTrue();
    }

    [Fact]
    public async Task AndCatchAsync_ShouldCatchException()
    {
        var expected = new InvalidOperationException();
        Exception? actual = null;
        Task asyncAction()
        {
            throw expected!;
        }

        await Execute.AndCatchAsync(asyncAction, (Exception ex) => actual = ex);

        actual.Should().Be(expected);
    }

    [Fact]
    public async Task AndCatchAsync_ShouldReturnResult()
    {
        var expected = Guid.NewGuid();
        Task<Guid> asyncAction() => Task.FromResult(expected);

        var actual = await Execute.AndCatchAsync(asyncAction, (Exception _) => { false.Should().BeTrue(); return Guid.Empty; });

        actual.Should().Be(expected);
    }

    [Fact]
    public async Task AndCatchAsync_ShouldCatchExceptionAndReturnResult()
    {
        var expected = new InvalidOperationException();
        var expectedResult = Guid.NewGuid();
        Exception? actual = null;
        Task<Guid> asyncAction()
        {
            throw expected!;
        }

        var actualResult = await Execute.AndCatchAsync(asyncAction, (Exception ex) => { actual = ex; return expectedResult; });

        actual.Should().Be(expected);
        actualResult.Should().Be(expectedResult);
    }

    [Fact]
    public void AndCatch_ShouldReturnResult()
    {
        var expected = Guid.NewGuid();

        var actual = Execute.AndCatch(() => expected, (Exception _) => { false.Should().BeTrue(); return Guid.Empty; });

        actual.Should().Be(expected);
    }

    [Fact]
    public void AndCatch_ShouldCatchExceptionAndReturnResult()
    {
        var expected = new InvalidOperationException();
        var expectedResult = Guid.NewGuid();
        Exception? actual = null;
        Guid asyncAction() => throw expected!;

        var actualResult = Execute.AndCatch(asyncAction, (Exception ex) => { actual = ex; return expectedResult; });

        actual.Should().Be(expected);
        actualResult.Should().Be(expectedResult);
    }
}
