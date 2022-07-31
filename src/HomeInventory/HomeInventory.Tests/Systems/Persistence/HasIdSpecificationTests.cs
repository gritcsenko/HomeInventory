using AutoFixture;
using FluentAssertions;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Customizations;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class HasIdSpecificationTests : BaseTest
{
    public HasIdSpecificationTests()
    {
        Fixture.Customize(new UserIdCustomization());
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
