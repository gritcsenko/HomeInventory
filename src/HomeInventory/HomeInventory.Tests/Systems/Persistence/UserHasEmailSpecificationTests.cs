using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class UserHasEmailSpecificationTests : BaseTest
{
    public UserHasEmailSpecificationTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();
    }

    [Fact]
    public void Should_SatisfyWithCorrectEmail()
    {
        var email = new Email(Fixture.Create<string>());
        var user = Fixture.Build<User>()
            .With(x => x.Email, email)
            .Create();
        var sut = new UserHasEmailSpecification(email);

        var actual = sut.IsSatisfiedBy(user);

        actual.Should().BeTrue();
    }

    [Fact]
    public void Should_NotSatisfyWithWrongEmail()
    {
        var user = Fixture.Build<User>()
            .With(x => x.Email, new Email(Fixture.Create<string>()))
            .Create();
        var sut = new UserHasEmailSpecification(new Email(Fixture.Create<string>()));

        var actual = sut.IsSatisfiedBy(user);

        actual.Should().BeFalse();
    }
}
