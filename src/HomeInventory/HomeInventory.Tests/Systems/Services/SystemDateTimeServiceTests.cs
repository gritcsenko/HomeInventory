using FluentAssertions;
using HomeInventory.Infrastructure.Services;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Systems.Services;

[Trait("Category", "Unit")]
public class SystemDateTimeServiceTests : BaseTest
{
    [Fact]
    public void Now_ShouldReturnCurrentTime()
    {
        var sut = new SystemDateTimeService();
        var expected = DateTimeOffset.Now;

        var actual = sut.Now;

        actual.Should().BeCloseTo(expected, TimeSpan.FromSeconds(1));
    }
}
