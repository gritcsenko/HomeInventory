using System.Diagnostics.CodeAnalysis;
using HomeInventory.Web.ErrorHandling;

namespace HomeInventory.Tests.Systems.ErrorHandling;

[UnitTest]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public class JsonWriterOptionsTests() : BaseTest<JsonWriterOptionsTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void Indented_ShouldHaveIndentedTrue()
    {
        Given.New(out var optionsVar, static () => JsonWriterOptions.Indented);

        var then = When
            .Invoked(optionsVar, static options => options.Indented);

        then
            .Result(static result => result.Should().BeTrue());
    }
}

public sealed class JsonWriterOptionsTestsGivenContext(BaseTest test) : GivenContext<JsonWriterOptionsTestsGivenContext>(test);

