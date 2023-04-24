using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Domain;

[UnitTest]
public class EquatableComponentTests : BaseTest
{
    [Fact]
    public void GetHashCode_ShoudReturnZero_WhenNoComponents()
    {
        var sut = new EquatableComponent<string>();

        var actual = sut.GetHashCode();

        actual.Should().Be(new HashCode().ToHashCode());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetHashCode_ShoudReturnCombinedComponentsHash_WhenManyComponents(int count)
    {
        var components = Fixture.CreateMany<Guid>(count).Cast<object>().ToArray();
        var expected = new HashCode();
        foreach (var component in components)
        {
            expected.Add(component);
        }
        var sut = new EquatableComponent<string>(components);

        var actual = sut.GetHashCode();

        actual.Should().Be(expected.ToHashCode());
    }

    [Fact]
    public void Equals_ShoudBeEqualToEmpty_WhenNoComponents()
    {
        var sut = new EquatableComponent<string>();

        var actual = sut.Equals(new EquatableComponent<string>());

        actual.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Equals_ShoudNotBeEqualToEmpty_WhenManyComponents(int count)
    {
        var components = Fixture.CreateMany<Guid>(count).Cast<object>().ToArray();
        var sut = new EquatableComponent<string>(components);

        var actual = sut.Equals(new EquatableComponent<string>());

        actual.Should().BeFalse();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Equals_ShoudBeEqualToComponentWithSameItems_WhenManyComponents(int count)
    {
        var components = Fixture.CreateMany<Guid>(count).Cast<object>().ToArray();
        var sut = new EquatableComponent<string>(components);

        var actual = sut.Equals(new EquatableComponent<string>(components));

        actual.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Equals_ShoudNotBeEqualToComponentWithDifferentItems_WhenManyComponents(int count)
    {
        var components = Fixture.CreateMany<Guid>(count).Cast<object>().ToArray();
        var sut = new EquatableComponent<string>(components);

        var actual = sut.Equals(new EquatableComponent<string>(Fixture.CreateMany<Guid>(count).Cast<object>().ToArray()));

        actual.Should().BeFalse();
    }
}
