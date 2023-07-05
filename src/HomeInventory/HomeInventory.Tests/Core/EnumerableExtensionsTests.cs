namespace HomeInventory.Tests.Core;

[UnitTest]
public class EnumerableExtensionsTests : BaseTest
{
    [Fact]
    public void Concat_Should_AddItemAtTheEnd()
    {
        var totalCount = 3;
        var expected = Fixture.CreateMany<int>(totalCount).ToReadOnly();
        var source = expected.Take(totalCount - 1).ToReadOnly();
        var item = expected.Last();

        var actual = source.Concat(item);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void EmptyIfNull_Should_ReturnOriginal_When_NotNull()
    {
        var totalCount = 3;
        var expected = Fixture.CreateMany<int>(totalCount).ToReadOnly();

        var actual = expected.EmptyIfNull();

        actual.Should().BeSameAs(expected);
    }

    [Fact]
    public void EmptyIfNull_Should_ReturnEmpty_When_Null()
    {
        IEnumerable<int>? sut = null;

        var actual = sut.EmptyIfNull();

        actual.Should().BeEmpty();
    }
}
