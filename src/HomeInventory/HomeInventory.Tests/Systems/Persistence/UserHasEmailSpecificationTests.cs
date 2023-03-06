using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Entities;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class UserHasEmailSpecificationTests : BaseTest
{
    public UserHasEmailSpecificationTests()
    {
        Fixture.Customize(new UserIdCustomization());
    }

    [Fact]
    public void Should_SatisfyWithCorrectEmail()
    {
        var email = Fixture.Create<string>();
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
            .With(x => x.Email, Fixture.Create<string>())
            .Create();
        var sut = new UserHasEmailSpecification(Fixture.Create<string>());

        var actual = sut.IsSatisfiedBy(user);

        actual.Should().BeFalse();
    }
}
