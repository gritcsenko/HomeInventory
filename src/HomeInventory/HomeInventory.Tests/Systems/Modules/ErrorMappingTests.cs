using System.Net;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Web.Infrastructure;

namespace HomeInventory.Tests.Systems.Modules;

[UnitTest]
public class ErrorMappingTests() : BaseTest<ErrorMappingTests.ErrorMappingTestsGivenContext>(t => new(t))
{
    [Fact]
    public void GetDefaultError_Shoud_Return500()
    {
        Given
            .Sut(out var sut);

        When
            .Invoked(sut, sut => sut.GetDefaultError())
            .Result(actual => actual.Should().Be(HttpStatusCode.InternalServerError));
    }

    [Theory]
    [ClassData<ErrorInstancesData>]
    public void GetError_Shoud_ReturnExpected_When_Error(Type? errorType, HttpStatusCode expected)
    {
        Given
            .Sut(out var sut);

        When
            .Invoked(sut, sut => sut.GetError(errorType))
            .Result(actual => actual.Should().Be(expected));
    }

#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class ErrorMappingTestsGivenContext(BaseTest test) : GivenContext<ErrorMappingTestsGivenContext>(test)
#pragma warning restore CA1034 // Nested types should not be visible
    {
        internal ErrorMappingTestsGivenContext Sut(out IVariable<ErrorMapping> sut) => New(out sut, ErrorMappingBuilder.CreateDefault().Build);
    }
}
