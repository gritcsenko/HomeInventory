using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class RepositoryTests : BaseRepositoryTest
{
    [Fact]
    public async ValueTask AddAsync_ShouldAdd()
    {
        var entity = Fixture.Create<User>();
        var sut = CreateSut();

        await sut.AddAsync(entity, Cancellation.Token);

        var actual = await Context.Set<UserModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().ContainSingle();
    }

    [Fact]
    public async ValueTask AddRangeAsync_ShouldAdd()
    {
        var entities = Fixture.CreateMany<User>();
        var sut = CreateSut();

        await sut.AddRangeAsync(entities, Cancellation.Token);

        var actual = await Context.Set<UserModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().HaveSameCount(entities);
    }

    [Fact]
    public async ValueTask DeleteAsync_ShouldRemoveExisting()
    {
        var entity = Fixture.Create<User>();
        var sut = CreateSut();
        await sut.AddAsync(entity, Cancellation.Token);

        await sut.DeleteAsync(entity, Cancellation.Token);

        var actual = await Context.Set<UserModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().BeEmpty();
    }

    [Fact]
    public async ValueTask DeleteRangeAsync_ShouldRemoveExisting()
    {
        var entities = Fixture.CreateMany<User>();
        var sut = CreateSut();
        await sut.AddRangeAsync(entities, Cancellation.Token);

        await sut.DeleteRangeAsync(entities, Cancellation.Token);

        var actual = await Context.Set<UserModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().HaveSameCount(entities);
    }

    [Fact]
    public async ValueTask FindFirstOptionalAsync_ShouldFindExisting()
    {
        var entity = Fixture.Create<User>();
        var sut = CreateSut();
        await sut.AddAsync(entity, Cancellation.Token);

        var actual = await sut.FindFirstOptionalAsync(new ByIdFilterSpecification<UserModel, UserId>(entity.Id), Cancellation.Token);

        actual.Should().HaveSomeValue();
    }

    [Fact]
    public async ValueTask FindFirstOptionalAsync_ShouldNotFindNonExisting()
    {
        Fixture.CustomizeUlidId<UserId>();
        var id = Fixture.Create<UserId>();
        var sut = CreateSut();

        var actual = await sut.FindFirstOptionalAsync(new ByIdFilterSpecification<UserModel, UserId>(id), Cancellation.Token);

        actual.Should().HaveNoValue();
    }

    [Fact]
    public async ValueTask HasAsync_ShouldFindExisting()
    {
        var entity = Fixture.Create<User>();
        var sut = CreateSut();
        await sut.AddAsync(entity, Cancellation.Token);

        var actual = await sut.HasAsync(new ByIdFilterSpecification<UserModel, UserId>(entity.Id), Cancellation.Token);

        actual.Should().BeTrue();
    }

    [Fact]
    public async ValueTask GetAllAsync_ShouldReturnExpected()
    {
        var model = Fixture.Create<UserModel>();
        var sut = CreateSut();
        await Context.Set<UserModel>().AddAsync(model, Cancellation.Token);

        var actual = await sut.GetAllAsync(Cancellation.Token).ToArrayAsync(Cancellation.Token);

        actual.Should().ContainSingle();
    }

    [Fact]
    public async ValueTask AnyAsync_ShouldReturnFalse_WhenNoModels()
    {
        var sut = CreateSut();

        var actual = await sut.AnyAsync(Cancellation.Token);

        actual.Should().BeFalse();
    }

    [Fact]
    public async ValueTask AnyAsync_ShouldReturnTrue_WhenModelsStored()
    {
        var model = Fixture.Create<UserModel>();
        var sut = CreateSut();
        await Context.Set<UserModel>().AddAsync(model, Cancellation.Token);

        var actual = await sut.AnyAsync(Cancellation.Token);

        actual.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async ValueTask CountAsync_ShouldReturnCorrectCount(int expectedCount)
    {
        var models = Fixture.CreateMany<UserModel>(expectedCount);
        var sut = CreateSut();
        await Context.Set<UserModel>().AddRangeAsync(models, Cancellation.Token);

        var actual = await sut.CountAsync(Cancellation.Token);

        actual.Should().Be(expectedCount);
    }

    private FakeRepository CreateSut() => new(Context, Mapper);

    private class FakeRepository : Repository<UserModel, User>
    {
        public FakeRepository(IDatabaseContext context, IMapper mapper)
            : base(context, mapper, SpecificationEvaluator.Default)
        {
        }
    }
}
