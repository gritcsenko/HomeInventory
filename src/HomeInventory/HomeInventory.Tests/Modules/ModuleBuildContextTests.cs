namespace HomeInventory.Tests.Modules;

[UnitTest]
public class ModuleBuildContextTests() : BaseTest<ModuleBuildContextTestsGivenContext>(t => new(t))
{
    [Fact]
    public void ApplicationBuilder_Should_ReturnApp()
    {
        Given
            .New<SubjectApp>(out var appVar, () => new())
            .Sut(out var sutVar, appVar);

        var then = When.Invoked(sutVar, sut => sut.ApplicationBuilder);

        then.Result(appVar, (actual, expected) => actual.Should().BeSameAs(expected));
    }

    [Fact]
    public void EndpointRouteBuilder_Should_ReturnApp()
    {
        Given
            .New<SubjectApp>(out var appVar, () => new())
            .Sut(out var sutVar, appVar);

        var then = When.Invoked(sutVar, sut => sut.EndpointRouteBuilder);

        then.Result(appVar, (actual, expected) => actual.Should().BeSameAs(expected));
    }
}