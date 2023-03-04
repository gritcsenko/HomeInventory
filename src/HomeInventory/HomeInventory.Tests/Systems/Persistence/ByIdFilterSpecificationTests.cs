﻿using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class ByIdFilterSpecificationTests : BaseDatabaseContextTest
{
    private readonly ISpecificationEvaluator _evaluator = SpecificationEvaluator.Default;
    private readonly Guid _id;
    private readonly IUnitOfWork _unitOfWork;

    public ByIdFilterSpecificationTests()
    {
        _id = Fixture.Create<Guid>();
        _unitOfWork = new UnitOfWork(Context, DateTimeService);
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

    [Fact]
    public async ValueTask ExecuteAsync_Should_SatisfyWithCorrectId()
    {
        var user = Fixture.Build<UserModel>()
            .With(m => m.Id, _id)
            .Create();
        var query = new[] { user }.AsQueryable();
        var sut = CreateSut();

        var actual = await sut.ExecuteAsync(_unitOfWork, CancellationToken);

        actual.Should().BeSameAs(user);
    }

    [Fact]
    public async ValueTask ExecuteAsync_Should_NotSatisfyWithWrongId()
    {
        var user = Fixture.Create<UserModel>();
        var query = new[] { user }.AsQueryable();
        var sut = CreateSut();

        var actual = await sut.ExecuteAsync(_unitOfWork, CancellationToken);

        actual.Should().NotBeSameAs(user);
    }

    private ByIdFilterSpecification<UserModel> CreateSut() => new(_id);
}