using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class ByIdFilterSpecificationTests : BaseDatabaseContextTest
{
    private readonly ISpecificationEvaluator _evaluator = SpecificationEvaluator.Default;
    private readonly UserId _id;
    private readonly IUnitOfWork _unitOfWork;

    public ByIdFilterSpecificationTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        _id = Fixture.Create<UserId>();
        _unitOfWork = new UnitOfWork(Context, DateTime, Substitute.For<IDisposable>());
    }

    [Fact]
    public void Should_SatisfyWithCorrectId()
    {
        var user = Fixture.Build<UserModel>()
            .With(m => m.Id, _id.Id)
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

    [Fact]
    public async ValueTask ExecuteAsync_Should_SatisfyWithCorrectId()
    {
        var user = Fixture.Build<UserModel>()
            .With(m => m.Id, _id.Id)
            .Create();
        var query = new[] { user }.AsQueryable();
        var sut = CreateSut();

        var actual = await sut.ExecuteAsync(_unitOfWork, Cancellation.Token);

        actual.Should().BeSameAs(user);
    }

    [Fact]
    public async ValueTask ExecuteAsync_Should_NotSatisfyWithWrongId()
    {
        var user = Fixture.Create<UserModel>();
        var sut = CreateSut();

        var actual = await sut.ExecuteAsync(_unitOfWork, Cancellation.Token);

        actual.Should().NotBeSameAs(user);
    }

    private ByIdFilterSpecification<UserModel> CreateSut() => new(_id.Id);
}
