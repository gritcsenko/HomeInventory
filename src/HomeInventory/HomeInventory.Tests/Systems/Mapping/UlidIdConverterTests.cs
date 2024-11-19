using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class UlidIdConverterTests() : BaseTest<UlidIdConverterTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void TryConvert_Should_ReturnValue_When_IdIsNotEmpty()
    {
        Given
            .Sut(out var sutVar)
            .New<Ulid>(out var idVar);

        var then = When
            .Invoked(sutVar, idVar, (sut, id) => sut.TryConvert(id));

        then
            .Result(idVar, (oneOf, id) => oneOf.Should().BeSuccess(x => x.Value.Should().Be(id)));
    }

    [Fact]
    public void TryConvert_Should_ReturnError_When_IdIsEmpty()
    {
        Given
            .Sut(out var sutVar)
            .Empty(out var idVar);

        var then = When
            .Invoked(sutVar, idVar, static (sut, id) => sut.TryConvert(id));

        then
            .Result(static validation => validation
                .Should().BeFail()
                .Which.Head.Should().BeOfType<ValidationError>()
                .Which.Value.Should().BeOfType<Ulid>()
                .Which.Should().BeEmpty());
    }

    [Fact]
    public void Convert_Should_Throw_When_IdIsEmpty()
    {
        Given
            .Sut(out var sutVar)
            .Empty(out var idVar);

        When
            .Catched(sutVar, idVar, static (sut, id) => sut.Convert(id))
            .Exception<ValidationException>(static ex => ex.Which.Value.Should().Be(Ulid.Empty));
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
        New(out empty, static () => Ulid.Empty);

    protected override UlidIdConverter<TestId> CreateSut() => new();
}

public class TestId(Ulid value) : UlidIdentifierObject<TestId>(value), IUlidBuildable<TestId>
{
    public static TestId CreateFrom(Ulid value) => new(value);
}
