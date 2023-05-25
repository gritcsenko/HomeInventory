using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class ByIdFilterSpecificationTests : BaseDatabaseContextTest
{
    private readonly ISpecificationEvaluator _evaluator = SpecificationEvaluator.Default;
    private readonly UserId _id;

    public ByIdFilterSpecificationTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        _id = Fixture.Create<UserId>();
    }

    [Fact]
    public void Should_SatisfyWithCorrectId()
    {
        var user = Fixture.Build<UserModel>()
            .With(m => m.Id, _id)
            .Create();
        var query = new[] { user }.AsQueryable();
        var sut = CreateSut();

        var actual = _evaluator.GetQuery(query, sut).Any();

        actual.Should().BeTrue();
    }

    [Fact]
    public void Should_NotSatisfyWithWrongId()
    {
        var user = Fixture.Create<UserModel>();
        var query = new[] { user }.AsQueryable();
        var sut = CreateSut();

        var actual = _evaluator.GetQuery(query, sut).Any();

        actual.Should().BeFalse();
    }

    private ByIdFilterSpecification<UserModel, UserId> CreateSut() => new(_id);
}
