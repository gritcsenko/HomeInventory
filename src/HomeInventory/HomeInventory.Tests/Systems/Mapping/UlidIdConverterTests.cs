using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class UlidIdConverterTests() : BaseTest<UlidIdConverterTestsGivenContext>(t => new(t))
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
            .Result(id, (oneOf, id) => oneOf.Should().BeSuccess(x => x.Value.Should().Be(id)));
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
            .Result(validation => validation
                .Should().BeFail()
                .Which.Head.Should().BeOfType<ValidationError>()
                .Which.Value.Should().BeOfType<Ulid>()
                .Which.Should().BeEmpty());
    }

    [Fact]
    public void Convert_Should_Throw_When_IdIsEmpty()
    {
        Given
            .Sut(out var sut)
            .Empty(out var id);

        When
            .Catched(sut, id, (sut, id) => sut.Convert(id))
            .Exception<ValidationException>(ex => ex.Which.Value.Should().Be(Ulid.Empty));
    }
}

public sealed class UlidIdConverterTestsGivenContext : GivenContext<UlidIdConverterTestsGivenContext, UlidIdConverter<TestId>>
{
    public UlidIdConverterTestsGivenContext(BaseTest test)
        : base(test)
    {
        test.Fixture.CustomizeUlid();
    }

    internal UlidIdConverterTestsGivenContext Empty(out IVariable<Ulid> empty) =>
        New(out empty, () => Ulid.Empty);

    protected override UlidIdConverter<TestId> CreateSut() => new();
}

public class TestId(Ulid value) : UlidIdentifierObject<TestId>(value), IUlidBuildable<TestId>
{
    public static TestId CreateFrom(Ulid value) => new(value);
}
