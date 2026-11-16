namespace HomeInventory.Tests.Core;

[UnitTest]
public sealed class DictionaryExtensionsTests : BaseTest
{
    [Fact]
    public void GetValueOrNone_ShouldReturnValue_WhenKeyExists()
    {
        var key = Fixture.Create<string>();
        var expected = Fixture.Create<Guid>();
        IReadOnlyDictionary<string, Guid> dictionary = new Dictionary<string, Guid> { [key] = expected };

        var actual = dictionary.GetValueOrNone(key);

        actual.Should().Be(expected);
    }

    [Fact]
    public void GetValueOrNone_ShouldReturnNone_WhenKeyIsMissing()
    {
        IReadOnlyDictionary<string, Guid> dictionary = new Dictionary<string, Guid> { [Fixture.Create<string>()] = Fixture.Create<Guid>() };

        var actual = dictionary.GetValueOrNone(Fixture.Create<string>());

        actual.Should().BeNone();
    }
}
