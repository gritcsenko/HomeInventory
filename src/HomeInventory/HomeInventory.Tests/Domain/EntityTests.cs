using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Tests.Domain;

[UnitTest]
public sealed class EntityTests() : BaseTest<EntityTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void EqualsTEntity_Should_ReturnTrueWhenSameReference()
    {
        Given
            .Id(out var idVar)
            .Sut(out var sutVar, idVar);

        When
            .Invoked(sutVar, static sut => sut.Equals(sut))
            .Result(static actual => actual.Should().BeTrue());
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnTrueWhenOtherHasSameId()
    {
        Given
            .Id(out var idVar)
            .Sut(out var otherVar, idVar)
            .Sut(out var sutVar, idVar);

        When
            .Invoked(sutVar, otherVar, static (sut, other) => sut.Equals(other))
            .Result(static actual => actual.Should().BeTrue());
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnFalseWhenOtherIsNull()
    {
        Given
            .Id(out var idVar)
            .Sut(out var sutVar, idVar);

        When
            .Invoked(sutVar, static sut => sut.Equals(default))
            .Result(static actual => actual.Should().BeFalse());
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        Given
            .Id(out var idVar, 2)
            .Sut(out var otherVar, idVar[0])
            .Sut(out var sutVar, idVar[1]);

        When
            .Invoked(sutVar, otherVar, static (sut, other) => sut.Equals(other))
            .Result(static actual => actual.Should().BeFalse());
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherHasDifferentType()
    {
        Given
            .Id(out var idVar)
            .New<object>(out var otherVar)
            .Sut(out var sutVar, idVar);

        When
            .Invoked(sutVar, otherVar, static (sut, other) => sut.Equals(other))
            .Result(static actual => actual.Should().BeFalse());
    }

    [Fact]
    public void EqualsObject_Should_ReturnTrueWhenSameReference()
    {
        Given
            .Id(out var idVar)
            .Sut(out var sutVar, idVar);

        When
            .Invoked(sutVar, static sut => sut.Equals((object)sut))
            .Result(static actual => actual.Should().BeTrue());
    }

    [Fact]
    public void EqualsObject_Should_ReturnTrueWhenOtherHasSameId()
    {
        Given
            .Id(out var idVar)
            .Sut(out var otherVar, idVar)
            .Sut(out var sutVar, idVar);

        When
            .Invoked(sutVar, otherVar, static (sut, other) => sut.Equals((object)other))
            .Result(static actual => actual.Should().BeTrue());
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherIsNull()
    {
        Given
            .Id(out var idVar)
            .Sut(out var sutVar, idVar);

        When
            .Invoked(sutVar, static sut => sut.Equals(default(object?)))
            .Result(static actual => actual.Should().BeFalse());
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        Given
            .Id(out var idVar, 2)
            .Sut(out var otherVar, idVar[0])
            .Sut(out var sutVar, idVar[1]);

        When
            .Invoked(sutVar, otherVar, static (sut, other) => sut.Equals((object)other))
            .Result(static actual => actual.Should().BeFalse());
    }

    [Fact]
    public void GetHashCode_Should_ReturnGetHashCodeFromId()
    {
        Given
            .Id(out var idVar)
            .AddAllToHashCode(out var hashVar, idVar)
            .Sut(out var sutVar, idVar);

        When
            .Invoked(sutVar, static sut => sut.GetHashCode())
            .Result(hashVar, static (actual, hash) => actual.Should().Be(hash.ToHashCode()));
    }

    [Fact]
    public void OpEquals_Should_ReturnTrueWhenOtherHasSameId()
    {
        Given
            .Id(out var idVar)
            .Sut(out var otherVar, idVar)
            .Sut(out var sutVar, idVar);

        When
            .Invoked(sutVar, otherVar, static (sut, other) => sut == other)
            .Result(static actual => actual.Should().BeTrue());
    }

    [Fact]
    public void OpEquals_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        Given
            .Id(out var idVar, 2)
            .Sut(out var otherVar, idVar[0])
            .Sut(out var sutVar, idVar[1]);

        When
            .Invoked(sutVar, otherVar, static (sut, other) => sut == other)
            .Result(static actual => actual.Should().BeFalse());
    }

    [Fact]
    public void OpNotEquals_Should_ReturnFalseWhenOtherHasSameId()
    {
        Given
            .Id(out var idVar)
            .Sut(out var otherVar, idVar)
            .Sut(out var sutVar, idVar);

        When
            .Invoked(sutVar, otherVar, static (sut, other) => sut != other)
            .Result(static actual => actual.Should().BeFalse());
    }

    [Fact]
    public void OpNotEquals_Should_ReturnTrueWhenOtherHasDifferentId()
    {
        Given
            .Id(out var idVar, 2)
            .Sut(out var otherVar, idVar[0])
            .Sut(out var sutVar, idVar[1]);

        When
            .Invoked(sutVar, otherVar, static (sut, other) => sut != other)
            .Result(static actual => actual.Should().BeTrue());
    }
}

public sealed class EntityTestsGivenContext : GivenContext<EntityTestsGivenContext, TestEntity, TestEntityId>
{
    public EntityTestsGivenContext(BaseTest test)
        : base(test) =>
        test.Fixture.CustomizeUlid();

    internal EntityTestsGivenContext Id(out IVariable<TestEntityId> id, int count = 1) => New(out id, CreateTestEntityId, count);

    protected override TestEntity CreateSut(TestEntityId arg) => new(arg);

    private TestEntityId CreateTestEntityId() => TestEntityId.CreateFrom(Create<Ulid>());
}

public class TestEntityId(Ulid value) : UlidIdentifierObject<TestEntityId>(value), IUlidBuildable<TestEntityId>
{
    public static TestEntityId CreateFrom(Ulid value) => new(value);
}

public class TestEntity(TestEntityId id) : Entity<TestEntity, TestEntityId>(id)
{
}
