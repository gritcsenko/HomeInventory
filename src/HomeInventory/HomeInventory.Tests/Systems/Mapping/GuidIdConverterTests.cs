using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class GuidIdConverterTests() : BaseTest<GuidIdConverterTestsGivenContext>(t => new(t))
{
    [Fact]
    public void TryConvert_Should_ReturnValue_When_IdIsNotEmpty()
    {
        Given
            .Sut(out var sut)
            .New<Ulid>(out var id);

        var then = When
            .Invoked(sut, id, (sut, id) => sut.TryConvert(id));

        then
            .Result(id, (oneOf, id) => oneOf.AsT0.Value.Should().Be(id));
    }

    [Fact]
    public void TryConvert_Should_ReturnError_When_IdIsEmpty()
    {
        Given
            .Sut(out var sut)
            .Empty(out var id);

        var then = When
            .Invoked(sut, id, (sut, id) => sut.TryConvert(id));

        then
            .Result(oneOf => oneOf.AsT1
                .Should().BeOfType<ObjectValidationError<Ulid>>()
                .Which.Value.Should().BeEmpty());
    }

    [Fact]
    public void Convert_Should_Throw_When_IdIsEmpty()
    {
        Given
            .Sut(out var sut)
            .Empty(out var id);

        When
            .Catched(sut, id, (sut, id) => sut.Convert(id))
            .Exception<InvalidOperationException>(ex => ex.Which.Data.ShouldBeDictionaryAnd().Contain(nameof(ObjectValidationError<Ulid>.Value), Ulid.Empty));
    }
}

public sealed class GuidIdConverterTestsGivenContext(BaseTest test) : GivenContext<GuidIdConverterTestsGivenContext, UlidIdConverter<UserId>>(test)
{
    internal GuidIdConverterTestsGivenContext Empty(out IVariable<Ulid> empty) =>
        New(out empty, () => Ulid.Empty);

    protected override UlidIdConverter<UserId> CreateSut() => new();
}
