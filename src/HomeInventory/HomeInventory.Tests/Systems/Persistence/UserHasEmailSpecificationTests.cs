using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoFixture;
using FluentAssertions;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class UserHasEmailSpecificationTests : BaseTest
{
    private readonly ISpecificationEvaluator _evaluator = new SpecificationEvaluator();

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
        var user = Fixture.Build<UserModel>()
            .With(x => x.Email, Fixture.Create<string>())
            .Create();
        var query = new[] { user }.AsQueryable();
        var sut = new UserHasEmailSpecification(new Email(Fixture.Create<string>()));

        var actual = _evaluator.GetQuery(query, sut).Any();

        actual.Should().BeFalse();
    }
}
