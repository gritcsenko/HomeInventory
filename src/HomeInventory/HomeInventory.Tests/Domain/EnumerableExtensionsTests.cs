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

    [Fact]
    public void Except_Should_AddRemoveItem()
    {
        var totalCount = 10;
        var source = Fixture.CreateMany<int>(totalCount).ToArray();
        var item = source.FirstRandom();

        var actual = source.Except(item);

        actual.Should().NotContain(item);
    }

    [Fact]
    public void Except_Should_AddRemoveItems()
    {
        var totalCount = 10;
        var source = Fixture.CreateMany<int>(totalCount).ToArray();
        var item1 = source.FirstRandom();
        var item2 = source.FirstRandom();

        var actual = source.Except(item1, item2);

        actual.Should().NotContain(item1);
        actual.Should().NotContain(item2);
    }
}
