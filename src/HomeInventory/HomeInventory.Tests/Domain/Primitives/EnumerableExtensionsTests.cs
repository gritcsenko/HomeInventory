using AutoFixture;
using FluentAssertions;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Domain;

public class EnumerableExtensionsTests : BaseTest
{
    [Fact]
    public void Concat_Should_AddItemAtTheEnd()
    {
        var totalCount = 3;
        var expected = Fixture.CreateMany<int>(totalCount).ToArray();
        var source = expected.Take(totalCount - 1).ToArray();
        var item = expected.Last();

        var actual = source.Concat(item);

        actual.Should().BeEquivalentTo(expected);
    }
}
