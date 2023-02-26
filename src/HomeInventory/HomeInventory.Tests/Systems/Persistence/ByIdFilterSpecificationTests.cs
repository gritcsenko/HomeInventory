using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class ByIdFilterSpecificationTests : BaseTest
{
    private readonly ISpecificationEvaluator _evaluator = SpecificationEvaluator.Default;

    [Fact]
    public void Should_SatisfyWithCorrectId()
    {
        var user = Fixture.Create<UserModel>();
        var id = user.Id;
        var query = new[] { user }.AsQueryable();
        var sut = new ByIdFilterSpecification<UserModel>(id);

        var actual = _evaluator.GetQuery(query, sut).Any();

        actual.Should().BeTrue();
    }

    [Fact]
    public void Should_NotSatisfyWithWrongId()
    {
        var user = Fixture.Create<UserModel>();
        var id = Fixture.Create<Guid>();
        var query = new[] { user }.AsQueryable();
        var sut = new ByIdFilterSpecification<UserModel>(id);

        var actual = _evaluator.GetQuery(query, sut).Any();

        actual.Should().BeFalse();
    }
}
