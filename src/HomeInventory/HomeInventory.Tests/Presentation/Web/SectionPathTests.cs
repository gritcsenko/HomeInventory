using HomeInventory.Web.Framework;

namespace HomeInventory.Tests.Presentation.Web;

[UnitTest]
public sealed class SectionPathTests() : BaseTest<SectionPathTests.GivenTestContext>(t => new(t))
{
    private static readonly Variable<SectionPath> _sut = new(nameof(_sut));
    private static readonly Variable<string> _path = new(nameof(_path));
    private static readonly Variable<string> _subPath = new(nameof(_subPath));

    [Fact]
    public void ToString_Should_ReturnPath()
    {
        Given
            .New(_path)
            .Sut(_sut, _path);

        var then = When
            .Invoked(_sut, sut => sut.ToString());

        then
            .Result(_path, (actual, expected) => actual.Should().Be(expected));
    }

    [Fact]
    public void Divide_Should_ReturnCombinedPath()
    {
        Given
            .New(_path)
            .New(_subPath)
            .Sut(_sut, _path);

        var then = When
            .Invoked(_sut, _subPath, (sut, subPath) => SectionPath.Divide(sut, subPath).ToString());

        then
            .Result(_path, _subPath, (actual, path, subPath) => actual.Should().Be($"{path}:{subPath}"));
    }

#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class GivenTestContext(BaseTest test) : GivenContext<GivenTestContext>(test)
#pragma warning restore CA1034 // Nested types should not be visible
    {
        internal GivenTestContext Sut(IVariable<SectionPath> sut, IVariable<string> pathVariable)
        {
            var path = GetValue(pathVariable);
            Add(sut, () => new(path));
            return this;
        }
    }
}
