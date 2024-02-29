using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Domain;

[UnitTest]
public class EntityTests : BaseTest<EntityTests.GivenTestContext>
{
    private static readonly Variable<TestEntityId> _id = new(nameof(_id));
    private static readonly Variable<TestEntity> _other = new(nameof(_other));
    private static readonly Variable<TestEntity> _sut = new(nameof(_sut));
    private static readonly Variable<HashCode> _hash = new(nameof(_hash));

    [Fact]
    public void EqualsTEntity_Should_ReturnTrueWhenSameReference()
    {
        Given
            .New(_id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, sut => sut.Equals(sut))
            .Result(actual => actual.Should().BeTrue());
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnTrueWhenOtherHasSameId()
    {
        Given
            .New(_id)
            .TestEntity(_other, _id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, _other, (sut, other) => sut.Equals(other))
            .Result(actual => actual.Should().BeTrue());
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnFalseWhenOtherIsNull()
    {
        Given
            .New(_id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, sut => sut.Equals(default))
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        Given
            .New(_id)
            .New(_id)
            .TestEntity(_other, _id[0])
            .TestEntity(_sut, _id[1]);

        When
            .Invoked(_sut, _other, (sut, other) => sut.Equals(other))
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherHasDifferentType()
    {
        Given
            .New(_id)
            .New(_other.OfType<object>())
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, _other.OfType<object>(), (sut, other) => sut.Equals(other))
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void EqualsObject_Should_ReturnTrueWhenSameReference()
    {
        Given
            .New(_id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, sut => sut.Equals((object)sut))
            .Result(actual => actual.Should().BeTrue());
    }

    [Fact]
    public void EqualsObject_Should_ReturnTrueWhenOtherHasSameId()
    {
        Given
            .New(_id)
            .TestEntity(_other, _id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, _other, (sut, other) => sut.Equals((object)other))
            .Result(actual => actual.Should().BeTrue());
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherIsNull()
    {
        Given
            .New(_id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, sut => sut.Equals(default(object?)))
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        Given
            .New(_id)
            .New(_id)
            .TestEntity(_other, _id[0])
            .TestEntity(_sut, _id[1]);

        When
            .Invoked(_sut, _other, (sut, other) => sut.Equals((object)other))
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void GetHashCode_Should_ReturnGetHashCodeFromId()
    {
        Given
            .New(_id)
            .AddAllToHashCode(_hash, _id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, sut => sut.GetHashCode())
            .Result(_hash, (actual, hash) => actual.Should().Be(hash.ToHashCode()));
    }

    [Fact]
    public void OpEquals_Should_ReturnTrueWhenOtherHasSameId()
    {
        Given
            .New(_id)
            .TestEntity(_other, _id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, _other, (sut, other) => sut == other)
            .Result(actual => actual.Should().BeTrue());
    }

    [Fact]
    public void OpEquals_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        Given
            .New(_id)
            .New(_id)
            .TestEntity(_other, _id[0])
            .TestEntity(_sut, _id[1]);

        When
            .Invoked(_sut, _other, (sut, other) => sut == other)
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void OpNotEquals_Should_ReturnFalseWhenOtherHasSameId()
    {
        Given
            .New(_id)
            .TestEntity(_other, _id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, _other, (sut, other) => sut != other)
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void OpNotEquals_Should_ReturnTrueWhenOtherHasDifferentId()
    {
        Given
            .New(_id)
            .New(_id)
            .TestEntity(_other, _id[0])
            .TestEntity(_sut, _id[1]);

        When
            .Invoked(_sut, _other, (sut, other) => sut != other)
            .Result(actual => actual.Should().BeTrue());
    }

    protected sealed override GivenTestContext CreateGiven(VariablesContainer variables) =>
        new(variables, Fixture);

#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class GivenTestContext : GivenContext<GivenTestContext>
#pragma warning restore CA1034 // Nested types should not be visible
    {
        public GivenTestContext(VariablesContainer variables, IFixture fixture)
            : base(variables, fixture)
        {
        }

        internal GivenTestContext TestEntity(IVariable<TestEntity> entity, IVariable<TestEntityId> id) =>
            Add(entity, () => CreateTestEntity(id));

        private TestEntity CreateTestEntity(IVariable<TestEntityId> id) => new(Variables.Get(id));
    }

    internal class TestEntityId : UlidIdentifierObject<TestEntityId>
    {
        public TestEntityId(Ulid value)
            : base(value)
        {
        }
    }

    internal class TestEntity : Entity<TestEntity, TestEntityId>
    {
        public TestEntity(TestEntityId id)
            : base(id)
        {
        }
    }
}
