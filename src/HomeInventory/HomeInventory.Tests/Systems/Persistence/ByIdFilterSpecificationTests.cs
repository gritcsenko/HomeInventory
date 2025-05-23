﻿using Ardalis.Specification.EntityFrameworkCore;
using HomeInventory.Domain.UserManagement.ValueObjects;
using HomeInventory.Infrastructure.Framework.Specifications;
using HomeInventory.Infrastructure.UserManagement.Models;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class ByIdFilterSpecificationTests : BaseDatabaseContextTest
{
    private readonly SpecificationEvaluator _evaluator = SpecificationEvaluator.Default;
    private readonly UserId _id;

    public ByIdFilterSpecificationTests()
    {
        Fixture.CustomizeId<UserId>();
        _id = Fixture.Create<UserId>();
    }

    [Fact]
    public void Should_SatisfyWithCorrectId()
    {
        var user = Fixture.Build<UserModel>()
            .With(static m => m.Id, _id)
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
    public async Task ExecuteAsync_Should_SatisfyWithCorrectId()
    {
        var user = Fixture.Build<UserModel>()
            .With(static m => m.Id, _id)
            .Create();
        await Context.Set<UserModel>().AddAsync(user, Cancellation.Token);
        await Context.SaveChangesAsync();
        var sut = CreateSut();

        var actual = await sut.ExecuteAsync(Context, Cancellation.Token);

        actual.Should().BeSameAs(user);
    }

    [Fact]
    public async Task ExecuteAsync_Should_NotSatisfyWithWrongId()
    {
        var user = Fixture.Create<UserModel>();
        await Context.Set<UserModel>().AddAsync(user, Cancellation.Token);
        await Context.SaveChangesAsync();
        var sut = CreateSut();

        var actual = await sut.ExecuteAsync(Context, Cancellation.Token);

        actual.Should().NotBeSameAs(user);
    }

    private ByIdFilterSpecification<UserModel, UserId> CreateSut() => new(_id);
}
