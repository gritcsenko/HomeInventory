using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class GuidIdConverterTests() : BaseTest<GuidIdConverterTests.GivenTestContext>(t => new(t))
{
    private static readonly Variable<UlidIdConverter<UserId>> _sut = new(nameof(_sut));
    private static readonly Variable<Ulid> _id = new(nameof(_id));

    [Fact]
    public void TryConvert_Should_ReturnValue_When_IdIsNotEmpty()
    {
        Given
            .Sut(_sut)
            .New(_id);

        var then = When
            .Invoked(_sut, _id, (sut, id) => sut.TryConvert(id));

        then
            .Result(_id, (oneOf, id) => oneOf.AsT0.Value.Should().Be(id));
    }

    [Fact]
    public void TryConvert_Should_ReturnError_When_IdIsEmpty()
    {
        Given
            .Sut(_sut)
            .Empty(_id);

        var then = When
            .Invoked(_sut, _id, (sut, id) => sut.TryConvert(id));

        then
            .Result(oneOf => oneOf.AsT1
                .Should().BeOfType<ObjectValidationError<Ulid>>()
                .Which.Value.Should().BeEmpty());
    }

    [Fact]
    public void Convert_Should_Throw_When_IdIsEmpty()
    {
        Given
            .Sut(_sut)
            .Empty(_id);

        When
            .Catched(_sut, _id, (sut, id) => sut.Convert(id))
            .Exception<InvalidOperationException>(ex => ex.Which.Data.ShouldBeDictionaryAnd().Contain(nameof(ObjectValidationError<Ulid>.Value), Ulid.Empty));
    }

#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class GivenTestContext(BaseTest test) : GivenContext<GivenTestContext>(test)
#pragma warning restore CA1034 // Nested types should not be visible
    {
        internal GivenTestContext Empty(Variable<Ulid> idVariable) => Add(idVariable, () => Ulid.Empty);

        internal GivenTestContext Sut(Variable<UlidIdConverter<UserId>> sutVariable) => Add(sutVariable, () => new());
    }
}
