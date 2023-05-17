using HomeInventory.Domain;

namespace HomeInventory.Tests.Systems.Services;

[UnitTest]
public class SystemDateTimeServiceTests : BaseTest
{
    [Fact]
    public void Now_ShouldReturnCurrentTime()
    {
        var sut = new SystemDateTimeService();
        var expected = DateTimeOffset.UtcNow;

        var actual = sut.UtcNow;

        actual.Should().BeCloseTo(expected, TimeSpan.FromSeconds(1));
    }
}
