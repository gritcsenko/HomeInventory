using System.Diagnostics.CodeAnalysis;
using HomeInventory.Domain.Errors;
using HomeInventory.Web.ErrorHandling;

namespace HomeInventory.Tests.Systems.ErrorHandling;

[UnitTest]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public class DataContractJsonConverterTests() : BaseTest<DataContractJsonConverterTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void CanConvert_WithAssignableType_ReturnsTrue()
    {
        Given
            .New<DataContractJsonConverter<Error>>(out var converterVar);

        var then = When
            .Invoked(converterVar, static converter => converter.CanConvert(typeof(InvalidCredentialsError)));

        then
            .Result(static result => result.Should().BeTrue());
    }

    [Fact]
    public void CanConvert_WithNonAssignableType_ReturnsFalse()
    {
        Given
            .New<DataContractJsonConverter<Error>>(out var converterVar);

        var then = When
            .Invoked(converterVar, static converter => converter.CanConvert(typeof(string)));

        then
            .Result(static result => result.Should().BeFalse());
    }
}

public sealed class DataContractJsonConverterTestsGivenContext(BaseTest test) : GivenContext<DataContractJsonConverterTestsGivenContext>(test);


