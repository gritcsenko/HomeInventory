using HomeInventory.Web.Framework;

namespace HomeInventory.Tests.Presentation.Web;

[UnitTest]
public sealed class SectionPathTests() : BaseTest<SectionPathTestsGivenContext>(t => new(t))
{
    [Fact]
    public void ToString_Should_ReturnPath()
    {
        Given
            .New<string>(out var path)
            .Sut(out var sut, path);

        var then = When
            .Invoked(sut, sut => sut.ToString());

        then
            .Result(path, (actual, expected) => actual.Should().Be(expected));
    }

    [Fact]
    public void Divide_Should_ReturnCombinedPath()
    {
        Given
            .New<string>(out var path)
            .New<string>(out var subPath)
            .Sut(out var sut, path);

        var then = When
            .Invoked(sut, subPath, (sut, subPath) => SectionPath.Divide(sut, subPath).ToString());

        then
            .Result(path, subPath, (actual, path, subPath) => actual.Should().Be($"{path}:{subPath}"));
    }
}

public sealed class SectionPathTestsGivenContext(BaseTest test) : GivenContext<SectionPathTestsGivenContext, SectionPath, string>(test)
{
    protected override SectionPath CreateSut(string arg) => new(arg);
}
