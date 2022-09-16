using AutoFixture;
using FluentAssertions;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Domain;
public class EntityTests : BaseTest
{
    [Fact]
    public void EqualsTEntity_Should_ReturnTrueWhenSameReference()
    {
        var id = Fixture.Create<EntityId>();
        var sut = new TestEntity(id);

        var result = sut.Equals(sut);

        result.Should().BeTrue();
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnTrueWhenOtherHasSameId()
    {
        var id = Fixture.Create<EntityId>();
        var sut = new TestEntity(id);
        var other = new TestEntity(id);

        var result = sut.Equals(other);

        result.Should().BeTrue();
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnFalseWhenOtherIsNull()
    {
        var id = Fixture.Create<EntityId>();
        var sut = new TestEntity(id);

        var result = sut.Equals(default(TestEntity?));

        result.Should().BeFalse();
    }

    [Fact]
    public void EqualsTEntity_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        var sut = new TestEntity(Fixture.Create<EntityId>());
        var other = new TestEntity(Fixture.Create<EntityId>());

        var result = sut.Equals(other);

        result.Should().BeFalse();
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherHasDifferentType()
    {
        var sut = new TestEntity(Fixture.Create<EntityId>());
        var other = new object();

        var result = sut.Equals(other);

        result.Should().BeFalse();
    }

    [Fact]
    public void EqualsObject_Should_ReturnTrueWhenSameReference()
    {
        var id = Fixture.Create<EntityId>();
        var sut = new TestEntity(id);

        var result = sut.Equals(sut);

        result.Should().BeTrue();
    }

    [Fact]
    public void EqualsObject_Should_ReturnTrueWhenOtherHasSameId()
    {
        var id = Fixture.Create<EntityId>();
        var sut = new TestEntity(id);
        object other = new TestEntity(id);

        var result = sut.Equals(other);

        result.Should().BeTrue();
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherIsNull()
    {
        var id = Fixture.Create<EntityId>();
        var sut = new TestEntity(id);

        var result = sut.Equals(default(object?));

        result.Should().BeFalse();
    }

    [Fact]
    public void EqualsObject_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        var sut = new TestEntity(Fixture.Create<EntityId>());
        object other = new TestEntity(Fixture.Create<EntityId>());

        var result = sut.Equals(other);

        result.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_Should_ReturnGetHashCodeFromId()
    {
        var id = Fixture.Create<EntityId>();
        var sut = new TestEntity(id);

        var result = sut.GetHashCode();

        result.Should().Be(id.GetHashCode());
    }

    [Fact]
    public void OpEquals_Should_ReturnTrueWhenOtherHasSameId()
    {
        var id = Fixture.Create<EntityId>();
        var sut = new TestEntity(id);
        var other = new TestEntity(id);

        var result = sut == other;

        result.Should().BeTrue();
    }

    [Fact]
    public void OpEquals_Should_ReturnFalseWhenOtherHasDifferentId()
    {
        var sut = new TestEntity(Fixture.Create<EntityId>());
        var other = new TestEntity(Fixture.Create<EntityId>());

        var result = sut == other;

        result.Should().BeFalse();
    }

    [Fact]
    public void OpNotEquals_Should_ReturnFalseWhenOtherHasSameId()
    {
        var id = Fixture.Create<EntityId>();
        var sut = new TestEntity(id);
        var other = new TestEntity(id);

        var result = sut != other;

        result.Should().BeFalse();
    }

    [Fact]
    public void OpNotEquals_Should_ReturnTrueWhenOtherHasDifferentId()
    {
        var sut = new TestEntity(Fixture.Create<EntityId>());
        var other = new TestEntity(Fixture.Create<EntityId>());

        var result = sut != other;

        result.Should().BeTrue();
    }

    private class EntityId : GuidIdentifierObject<EntityId>
    {
        public EntityId(Guid value)
            : base(value)
        {
        }
    }

    private class TestEntity : Entity<TestEntity, EntityId>
    {
        public TestEntity(EntityId id)
            : base(id)
        {
        }
    }
}
