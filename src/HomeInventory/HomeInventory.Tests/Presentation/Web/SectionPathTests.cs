﻿using HomeInventory.Web.Framework;

namespace HomeInventory.Tests.Presentation.Web;

[UnitTest]
public sealed class SectionPathTests() : BaseTest<SectionPathTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void ToString_Should_ReturnPath()
    {
        Given
            .New<string>(out var pathVar)
            .Sut(out var sutVar, pathVar);

        var then = When
            .Invoked(sutVar, static sut => sut.ToString());

        then
            .Result(pathVar, static (actual, expected) => actual.Should().Be(expected));
    }

    [Fact]
    public void Divide_Should_ReturnCombinedPath()
    {
        Given
            .New<string>(out var pathVar)
            .New<string>(out var subPathVar)
            .Sut(out var sutVar, pathVar);

        var then = When
            .Invoked(sutVar, subPathVar, static (sut, subPath) => SectionPath.Divide(sut, subPath).ToString());

        then
            .Result(pathVar, subPathVar, static (actual, path, subPath) => actual.Should().Be($"{path}:{subPath}"));
    }
}

public sealed class SectionPathTestsGivenContext(BaseTest test) : GivenContext<SectionPathTestsGivenContext, SectionPath, string>(test)
{
    protected override SectionPath CreateSut(string arg) => new(arg);
}
