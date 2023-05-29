using HomeInventory.Domain.Primitives;

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

    public sealed class PublicTestSubject
    {
        public PublicTestSubject()
        {
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public PublicTestSubject(object arg)
#pragma warning restore IDE0060 // Remove unused parameter
        {
        }
    }

    public sealed class InternalTestSubject
    {
        internal InternalTestSubject()
        {
        }

#pragma warning disable IDE0060 // Remove unused parameter
        internal InternalTestSubject(object arg)
#pragma warning restore IDE0060 // Remove unused parameter
        {
        }
    }
}
