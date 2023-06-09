using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Tests.Systems.Modules;

[UnitTest]
public class ErrorMappingTests : BaseTest<ErrorMappingTests.GivenContext>
{
    private static readonly Variable<ErrorMapping> _sut = new(nameof(_sut));

    [Fact]
    public void GetDefaultError_Shoud_Return500()
    {
        Given
            .Sut(_sut);

        When
            .Invoked(_sut, sut => sut.GetDefaultError())
            .Result(actual => actual.Should().Be(StatusCodes.Status500InternalServerError));
    }

    [Theory]
    [ClassData<ErrorInstancesData>]
    public void GetError_Shoud_ReturnExpected_When_Error(IError error, int expected)
    {
        Given
            .Sut(_sut);

        When
            .Invoked(_sut, sut => sut.GetError(error))
            .Result(actual => actual.Should().Be(expected));
    }

    [Fact]
    public void GetErrorT_Shoud_Return409_When_ConflictError()
    {
        Given
            .Sut(_sut);

        When
            .Invoked(_sut, sut => sut.GetError<ConflictError>())
            .Result(actual => actual.Should().Be(StatusCodes.Status409Conflict));
    }

    [Fact]
    public void GetErrorT_Shoud_Return409_When_DerivedFromConflictError()
    {
        Given
            .Sut(_sut);

        When
            .Invoked(_sut, sut => sut.GetError<DuplicateEmailError>())
            .Result(actual => actual.Should().Be(StatusCodes.Status409Conflict));
    }

    [Fact]
    public void GetErrorT_Shoud_Return403_When_InvalidCredentialsError()
    {
        Given
            .Sut(_sut);

        When
            .Invoked(_sut, sut => sut.GetError<InvalidCredentialsError>())
            .Result(actual => actual.Should().Be(StatusCodes.Status403Forbidden));
    }

    protected override GivenContext CreateGiven(VariablesContainer variables) => new(variables, Fixture);

#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class GivenContext : GivenContext<GivenContext>
#pragma warning restore CA1034 // Nested types should not be visible
    {
        public GivenContext(VariablesContainer variables, IFixture fixture)
             : base(variables, fixture)
        {
        }

        internal GivenContext Sut(IVariable<ErrorMapping> sut) => Add(sut, () => new());
    }
}
