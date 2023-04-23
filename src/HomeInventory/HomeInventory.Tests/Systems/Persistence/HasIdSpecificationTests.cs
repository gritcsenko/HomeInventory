using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class HasIdSpecificationTests : BaseTest
{
    public HasIdSpecificationTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();
    }

    [Fact]
    public void Should_SatisfyWithCorrectId()
    {
        var user = Fixture.Create<User>();
        var id = user.Id;
        var sut = new HasIdSpecification<User, UserId>(id);

        var actual = sut.IsSatisfiedBy(user);

        actual.Should().BeTrue();
    }

    [Fact]
    public void Should_NotSatisfyWithWrongId()
    {
        var user = Fixture.Create<User>();
        var id = Fixture.Create<UserId>();
        var sut = new HasIdSpecification<User, UserId>(id);

        var actual = sut.IsSatisfiedBy(user);

        actual.Should().BeFalse();
    }
}
