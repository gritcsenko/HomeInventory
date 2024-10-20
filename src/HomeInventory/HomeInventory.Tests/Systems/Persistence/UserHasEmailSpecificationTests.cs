using Ardalis.Specification.EntityFrameworkCore;
using HomeInventory.Domain.UserManagement.ValueObjects;
using HomeInventory.Infrastructure.UserManagement.Models;
using HomeInventory.Infrastructure.UserManagement.Specifications;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class UserHasEmailSpecificationTests : BaseTest
{
    private readonly SpecificationEvaluator _evaluator = SpecificationEvaluator.Default;

    public UserHasEmailSpecificationTests()
    {
        Fixture.CustomizeId<UserId>();
    }

    [Fact]
    public void Should_SatisfyWithCorrectEmail()
    {
        var email = Fixture.Create<string>();
        var user = Fixture.Build<UserModel>()
            .With(x => x.Email, email)
            .Create();
        var query = new[] { user }.AsQueryable();
        var sut = new UserHasEmailSpecification(new Email(email));

        var actual = _evaluator.GetQuery(query, sut).Any();

        actual.Should().BeTrue();
    }

    [Fact]
    public void Should_NotSatisfyWithWrongEmail()
    {
        var query = Fixture.Build<UserModel>()
            .With(x => x.Email, Fixture.Create<string>())
            .CreateMany()
            .AsQueryable();
        var sut = new UserHasEmailSpecification(new Email(Fixture.Create<string>()));

        var actual = _evaluator.GetQuery(query, sut).Any();

        actual.Should().BeFalse();
    }
}
