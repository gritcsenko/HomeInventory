namespace HomeInventory.Tests.Domain.Primitives;

[UnitTest]
public class ReflectionMethodsTests : BaseTest
{
    [Fact]
    public void CreateInstance_ShouldCreate_When_PublicCtorAndNoArgs()
    {
        var actual = ReflectionMethods.CreateInstance<PublicTestSubject>();

        actual.Should().NotBeNull();
    }

    [Fact]
    public void CreateInstance_ShouldCreate_When_PublicCtorAndArg()
    {
        var arg = Fixture.Create<object>();

        var actual = ReflectionMethods.CreateInstance<PublicTestSubject>(arg);

        actual.Should().NotBeNull();
    }

    [Fact]
    public void CreateInstance_ShouldCreate_When_InternalCtorAndNoArgs()
    {
        var actual = ReflectionMethods.CreateInstance<InternalTestSubject>();

        actual.Should().NotBeNull();
    }

    [Fact]
    public void CreateInstance_ShouldCreate_When_InternalCtorAndArg()
    {
        var arg = Fixture.Create<object>();

        var actual = ReflectionMethods.CreateInstance<InternalTestSubject>(arg);

        actual.Should().NotBeNull();
    }
}
