using AutoFixture;
using FluentAssertions;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Customizations;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class SpecificationExtensionsTests : BaseTest
{
    public SpecificationExtensionsTests()
    {
        Fixture.Customize(new UserIdCustomization());
    }

    [Fact]
    public void And_Should_Satisfy_WhenBothAreSatisfied()
    {
        var email = Fixture.Create<string>();
        var user = Fixture.Build<User>()
            .With(x => x.Email, email)
            .Create();
        var id = user.Id;
        var left = new UserHasEmailSpecification(email);
        var right = new HasIdSpecification<User, UserId>(id);
        var sut = left.And(right);

        var actual = sut.IsSatisfiedBy(user);

        actual.Should().BeTrue();
    }

    [Fact]
    public void Or_Should_Satisfy_WhenOneIsSatisfied()
    {
        var email = Fixture.Create<string>();
        var user = Fixture.Build<User>()
            .With(x => x.Email, email)
            .Create();
        var id = Fixture.Create<UserId>();
        var left = new UserHasEmailSpecification(email);
        var right = new HasIdSpecification<User, UserId>(id);
        var sut = left.Or(right);

        var actual = sut.IsSatisfiedBy(user);

        actual.Should().BeTrue();
    }

    [Fact]
    public void Not_Should_SatisfyWithNotSatisfiedOriginal()
    {
        var user = Fixture.Create<User>();
        var id = Fixture.Create<UserId>();
        var sut = new HasIdSpecification<User, UserId>(id).Not();

        var actual = sut.IsSatisfiedBy(user);

        actual.Should().BeTrue();
    }
}
