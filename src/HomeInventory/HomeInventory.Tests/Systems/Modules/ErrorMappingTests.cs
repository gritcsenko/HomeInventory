using System.Net;
using HomeInventory.Web.ErrorHandling;

namespace HomeInventory.Tests.Systems.Modules;

[UnitTest]
public class ErrorMappingTests() : BaseTest<ErrorMappingTests.ErrorMappingTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void GetDefaultError_Should_Return500()
    {
        Given
            .Sut(out var sutVar);

        When
            .Invoked(sutVar, static sut => sut.GetDefaultError())
            .Result(static actual => actual.Should().Be(HttpStatusCode.InternalServerError));
    }

    [Theory]
    [ClassData<ErrorInstancesData>]
    public void GetError_Should_ReturnExpected_When_Error(Type? errorType, HttpStatusCode expected)
    {
        Given
            .Sut(out var sutVar);

        When
            .Invoked(sutVar, sut => sut.GetError(errorType))
            .Result(actual => actual.Should().Be(expected));
    }

#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class ErrorMappingTestsGivenContext(BaseTest test) : GivenContext<ErrorMappingTestsGivenContext>(test)
#pragma warning restore CA1034 // Nested types should not be visible
    {
        internal ErrorMappingTestsGivenContext Sut(out IVariable<ErrorMapping> sut) => New(out sut, ErrorMappingBuilder.CreateDefault().Build);
    }
}
