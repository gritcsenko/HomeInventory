using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Tests.Domain;

[UnitTest]
public sealed class EntityTests() : BaseTest<EntityTestsGivenContext>(t => new(t))
{
    [Fact]
    public void EqualsTEntity_Should_ReturnTrueWhenSameReference()
    {
        Given
            .Id(out var id)
            .Sut(out var sut, id);

        When
            .Invoked(sut, sut => sut.Equals(sut))
            .Result(actual => actual.Should().BeTrue());
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnTrueWhenOtherHasSameId()
    {
        Given
            .Id(out var id)
            .Sut(out var other, id)
            .Sut(out var sut, id);

        When
            .Invoked(sut, other, (sut, other) => sut.Equals(other))
            .Result(actual => actual.Should().BeTrue());
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnFalseWhenOtherIsNull()
    {
        Given
            .Id(out var id)
            .Sut(out var sut, id);

        When
            .Invoked(sut, sut => sut.Equals(default))
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        Given
            .Id(out var id, 2)
            .Sut(out var other, id[0])
            .Sut(out var sut, id[1]);

        When
            .Invoked(sut, other, (sut, other) => sut.Equals(other))
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherHasDifferentType()
    {
        Given
            .Id(out var id)
            .New<object>(out var other)
            .Sut(out var sut, id);

        When
            .Invoked(sut, other, (sut, other) => sut.Equals(other))
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void EqualsObject_Should_ReturnTrueWhenSameReference()
    {
        Given
            .Id(out var id)
            .Sut(out var sut, id);

        When
            .Invoked(sut, sut => sut.Equals((object)sut))
            .Result(actual => actual.Should().BeTrue());
    }

    [Fact]
    public void EqualsObject_Should_ReturnTrueWhenOtherHasSameId()
    {
        Given
            .Id(out var id)
            .Sut(out var other, id)
            .Sut(out var sut, id);

        When
            .Invoked(sut, other, (sut, other) => sut.Equals((object)other))
            .Result(actual => actual.Should().BeTrue());
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherIsNull()
    {
        Given
            .Id(out var id)
            .Sut(out var sut, id);

        When
            .Invoked(sut, sut => sut.Equals(default(object?)))
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        Given
            .Id(out var id, 2)
            .Sut(out var other, id[0])
            .Sut(out var sut, id[1]);

        When
            .Invoked(sut, other, (sut, other) => sut.Equals((object)other))
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void GetHashCode_Should_ReturnGetHashCodeFromId()
    {
        Given
            .Id(out var id)
            .AddAllToHashCode(out var hash, id)
            .Sut(out var sut, id);

        When
            .Invoked(sut, sut => sut.GetHashCode())
            .Result(hash, (actual, hash) => actual.Should().Be(hash.ToHashCode()));
    }

    [Fact]
    public void OpEquals_Should_ReturnTrueWhenOtherHasSameId()
    {
        Given
            .Id(out var id)
            .Sut(out var other, id)
            .Sut(out var sut, id);

        When
            .Invoked(sut, other, (sut, other) => sut == other)
            .Result(actual => actual.Should().BeTrue());
    }

    [Fact]
    public void OpEquals_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        Given
            .Id(out var id, 2)
            .Sut(out var other, id[0])
            .Sut(out var sut, id[1]);

        When
            .Invoked(sut, other, (sut, other) => sut == other)
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void OpNotEquals_Should_ReturnFalseWhenOtherHasSameId()
    {
        Given
            .Id(out var id)
            .Sut(out var other, id)
            .Sut(out var sut, id);

        When
            .Invoked(sut, other, (sut, other) => sut != other)
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void OpNotEquals_Should_ReturnTrueWhenOtherHasDifferentId()
    {
        Given
            .Id(out var id, 2)
            .Sut(out var other, id[0])
            .Sut(out var sut, id[1]);

        When
            .Invoked(sut, other, (sut, other) => sut != other)
            .Result(actual => actual.Should().BeTrue());
    }
}

public sealed class EntityTestsGivenContext : GivenContext<EntityTestsGivenContext, TestEntity, TestEntityId>
{
    public EntityTestsGivenContext(BaseTest test)
        : base(test)
    {
        test.Fixture.CustomizeUlid();
    }

    internal EntityTestsGivenContext Id(out IVariable<TestEntityId> id, int count = 1) => New(out id, CreateTestEntityId, count);

    protected override TestEntity CreateSut(TestEntityId arg) => new(arg);

    private TestEntityId CreateTestEntityId() => TestEntityId.CreateFrom(Create<Ulid>()).Value;
}

public class TestEntityId(Ulid value) : UlidIdentifierObject<TestEntityId>(value), IUlidBuildable<TestEntityId>
{
    public static Optional<TestEntityId> CreateFrom(Ulid value) => new TestEntityId(value);
}

public class TestEntity(TestEntityId id) : Entity<TestEntity, TestEntityId>(id)
{
}
