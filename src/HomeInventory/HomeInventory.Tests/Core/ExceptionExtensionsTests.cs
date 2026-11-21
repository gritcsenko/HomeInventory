using ExceptionExtensions = HomeInventory.Core.ExceptionExtensions;

namespace HomeInventory.Tests.Core;

[UnitTest]
public sealed class ExceptionExtensionsTests
{
    [Fact]
    public void Rethrow_ShouldThrowSameException()
    {
        var expected = new InvalidOperationException();

        Action action = () => ExceptionExtensions.Rethrow(expected);

        action.Should().Throw<InvalidOperationException>().Which.Should().BeSameAs(expected);
    }

    [Fact]
    public void Rethrow_ShouldPreserveOriginalStackTrace()
    {
        var expected = Capture();
        var originalStackTrace = expected.StackTrace;

        Action action = () => ExceptionExtensions.Rethrow(expected);

        action.Should().Throw<InvalidOperationException>()
            .Which.StackTrace.Should().StartWith(originalStackTrace);
    }

    [Fact]
    public void RethrowOfT_ShouldThrowSameException()
    {
        var expected = new InvalidOperationException();

        Func<Guid> action = () => ExceptionExtensions.Rethrow<Guid>(expected);

        action.Should().Throw<InvalidOperationException>().Which.Should().BeSameAs(expected);
    }

    private static InvalidOperationException Capture()
    {
        try
        {
            throw new InvalidOperationException();
        }
        catch (InvalidOperationException e)
        {
            return e;
        }
    }
}
