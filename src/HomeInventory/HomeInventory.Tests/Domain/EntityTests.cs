using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Domain;

[UnitTest]
public class EntityTests : BaseTest<EntityTests.GivenTestContext, WhenContext, ThenContext>
{
    private static readonly Variable<EntityId> _id = new(nameof(_id));
    private static readonly Variable<TestEntity> _other = new(nameof(_other));
    private static readonly Variable<TestEntity> _sut = new(nameof(_sut));
    private static readonly Variable<HashCode> _hash = new(nameof(_hash));

    public EntityTests()
    {
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnTrueWhenSameReference()
    {
        Given
            .New(_id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, sut => sut.Equals(sut));

        Then
            .Result<bool>(x => x.Should().BeTrue());
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnTrueWhenOtherHasSameId()
    {
        Given
            .New(_id)
            .TestEntity(_other, _id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, _other, (sut, other) => sut.Equals(other));

        Then
            .Result<bool>(x => x.Should().BeTrue());
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnFalseWhenOtherIsNull()
    {
        Given
            .New(_id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, sut => sut.Equals(default(TestEntity?)));

        Then
            .Result<bool>(x => x.Should().BeFalse());
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        Given
            .New(_id)
            .New(_id)
            .TestEntity(_other, _id)
            .TestEntity(_sut, _id.WithIndex(1));

        When
            .Invoked(_sut, _other, (sut, other) => sut.Equals(other));

        Then
            .Result<bool>(x => x.Should().BeFalse());
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherHasDifferentType()
    {
        Given
            .New(_id)
            .New(_other.OfType<object>())
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, _other.OfType<object>(), (sut, other) => sut.Equals(other));

        Then
            .Result<bool>(x => x.Should().BeFalse());
    }

    [Fact]
    public void EqualsObject_Should_ReturnTrueWhenSameReference()
    {
        Given
            .New(_id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, sut => sut.Equals((object)sut));

        Then
            .Result<bool>(x => x.Should().BeTrue());
    }

    [Fact]
    public void EqualsObject_Should_ReturnTrueWhenOtherHasSameId()
    {
        Given
            .New(_id)
            .TestEntity(_other, _id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, _other, (sut, other) => sut.Equals((object)other));

        Then
            .Result<bool>(x => x.Should().BeTrue());
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherIsNull()
    {
        Given
            .New(_id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, sut => sut.Equals(default(object?)));

        Then
            .Result<bool>(x => x.Should().BeFalse());
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        Given
            .New(_id)
            .New(_id)
            .TestEntity(_other, _id)
            .TestEntity(_sut, _id.WithIndex(1));

        When
            .Invoked(_sut, _other, (sut, other) => sut.Equals((object)other));

        Then
            .Result<bool>(x => x.Should().BeFalse());
    }

    [Fact]
    public void GetHashCode_Should_ReturnGetHashCodeFromId()
    {
        Given
            .New(_id)
            .AddToHashCode(_hash, _id.WithIndex(0))
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, sut => sut.GetHashCode());

        Then
            .Result<int, EntityId>(_id, (x, id) => x.Should().Be(id.GetHashCode()));
    }

    [Fact]
    public void OpEquals_Should_ReturnTrueWhenOtherHasSameId()
    {
        Given
            .New(_id)
            .TestEntity(_other, _id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, _other, (sut, other) => sut == other);

        Then
            .Result<bool>(x => x.Should().BeTrue());
    }

    [Fact]
    public void OpEquals_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        Given
            .New(_id)
            .New(_id)
            .TestEntity(_other, _id)
            .TestEntity(_sut, _id.WithIndex(1));

        When
            .Invoked(_sut, _other, (sut, other) => sut == other);

        Then
            .Result<bool>(x => x.Should().BeFalse());
    }

    [Fact]
    public void OpNotEquals_Should_ReturnFalseWhenOtherHasSameId()
    {
        Given
            .New(_id)
            .TestEntity(_other, _id)
            .TestEntity(_sut, _id);

        When
            .Invoked(_sut, _other, (sut, other) => sut != other);

        Then
            .Result<bool>(x => x.Should().BeFalse());
    }

    [Fact]
    public void OpNotEquals_Should_ReturnTrueWhenOtherHasDifferentId()
    {
        Given
            .New(_id)
            .New(_id)
            .TestEntity(_other, _id)
            .TestEntity(_sut, _id.WithIndex(1));

        When
            .Invoked(_sut, _other, (sut, other) => sut != other);

        Then
            .Result<bool>(x => x.Should().BeTrue());
    }

    protected override GivenTestContext CreateGiven(VariablesCollection variables)
    {
        return new GivenTestContext(variables, Fixture);
    }

    protected override WhenContext CreateWhen(VariablesCollection variables)
    {
        return new WhenContext(variables, Result);
    }

    protected override ThenContext CreateThen(VariablesCollection variables)
    {
        return new ThenContext(variables, Result);
    }

    public sealed class GivenTestContext : GivenContext<GivenTestContext>
    {
        public GivenTestContext(VariablesCollection variables, IFixture fixture)
            : base(variables, fixture)
        {
        }

        public GivenTestContext TestEntity(IVariable<TestEntity> entity, IndexedVariable<EntityId> id) =>
            Add(entity, () => CreateTestEntity(id));

        private TestEntity CreateTestEntity(IIndexedVariable<EntityId> id) => new(Variables.Get(id));
    }

    public class EntityId : GuidIdentifierObject<EntityId>
    {
        public EntityId(Guid value)
            : base(value)
        {
        }
    }

    public class TestEntity : Entity<TestEntity, EntityId>
    {
        public TestEntity(EntityId id)
            : base(id)
        {
        }
    }
}
