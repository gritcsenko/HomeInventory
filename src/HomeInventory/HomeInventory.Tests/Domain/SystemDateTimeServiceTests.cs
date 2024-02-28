using FluentAssertions.Execution;

namespace HomeInventory.Tests.Domain;

[UnitTest]
public class SystemDateTimeServiceTests : BaseTest
{
    [Fact]
    public void UtcNow_ShouldReturnCurrentTime()
    {
        var sut = new SystemDateTimeService();
        var expected = DateTimeOffset.UtcNow;

        var actual = sut.UtcNow;

        using var scope = new AssertionScope();
        actual.Should().BeCloseTo(expected, TimeSpan.FromSeconds(1));
        actual.Offset.Should().Be(TimeSpan.Zero);
    }
}
