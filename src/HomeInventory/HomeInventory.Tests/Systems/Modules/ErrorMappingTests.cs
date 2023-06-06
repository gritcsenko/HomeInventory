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

    protected override GivenContext CreateGiven(VariablesCollection variables) => new(variables, Fixture);

    public sealed class GivenContext : GivenContext<GivenContext>
    {
        public GivenContext(VariablesCollection variables, IFixture fixture)
             : base(variables, fixture)
        {
        }

        internal GivenContext Sut(IVariable<ErrorMapping> sut) => Add(sut, () => new());
    }

    public sealed class ErrorInstancesData : TheoryData<IError, int>
    {
        public ErrorInstancesData()
        {
            Add(new ConflictError(""), StatusCodes.Status409Conflict);
            Add(new DuplicateEmailError(), StatusCodes.Status409Conflict);
            Add(new ValidationError(""), StatusCodes.Status400BadRequest);
            Add(new ObjectValidationError<string>(""), StatusCodes.Status400BadRequest);
            Add(new NotFoundError(""), StatusCodes.Status404NotFound);
        }
    }
}
