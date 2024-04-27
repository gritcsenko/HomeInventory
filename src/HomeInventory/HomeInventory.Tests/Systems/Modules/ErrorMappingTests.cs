using System.Net;
using HomeInventory.Web.Infrastructure;

namespace HomeInventory.Tests.Systems.Modules;

[UnitTest]
public class ErrorMappingTests() : BaseTest<ErrorMappingTests.GivenContext>(t => new(t))
{
    private static readonly Variable<ErrorMapping> _sut = new(nameof(_sut));

    [Fact]
    public void GetDefaultError_Shoud_Return500()
    {
        Given
            .Sut(_sut);

        When
            .Invoked(_sut, sut => sut.GetDefaultError())
            .Result(actual => actual.Should().Be(HttpStatusCode.InternalServerError));
    }

    [Theory]
    [ClassData<ErrorInstancesData>]
    public void GetError_Shoud_ReturnExpected_When_Error(Type? errorType, HttpStatusCode expected)
    {
        Given
            .Sut(_sut);

        When
            .Invoked(_sut, sut => sut.GetError(errorType))
            .Result(actual => actual.Should().Be(expected));
    }

#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class GivenContext(BaseTest test) : GivenContext<GivenContext>(test)
#pragma warning restore CA1034 // Nested types should not be visible
    {
        internal GivenContext Sut(IVariable<ErrorMapping> sut) => Add(sut, ErrorMappingBuilder.CreateDefault().Build);
    }
}
